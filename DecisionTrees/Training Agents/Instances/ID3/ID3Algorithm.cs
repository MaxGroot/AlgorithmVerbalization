using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ID3Algorithm : Algorithm
    {
        private List<DataInstance> examples;
        private string target_attribute;
        private List<string> all_attributes;
        private Dictionary<string, List<string>> possible_attribute_values = new Dictionary<string, List<string>>();
        private Agent agent;
    public DecisionTree train(List<DataInstance> examples, string target_attribute, Dictionary<string, string> attributes, Agent agent, Dictionary<string, object> parameters)
        {
            this.examples = examples;
            this.target_attribute = target_attribute;
            this.all_attributes = attributes.Keys.ToList();
            this.agent = agent;
            

            // First we need to know for each attribute which possible values it can hold.
            this.calculateAttributePossibilities();
            
            DecisionTree tree = new DecisionTree(target_attribute);

            // Start the iteration process on the entire set.
            agent.THINK("start").finish();
            tree = this.iterate(tree, this.examples, attributes.Keys.ToList(), null, null);
            agent.THINK("return-prediction-model").finish();
            return tree;
        }

        public DecisionTree iterate(DecisionTree tree, List<DataInstance> sets_todo, List<string> considerable_attributes, Node parent_node, string parent_value_splitter)
        {
            
            agent.THINK("iterate").finish();
            List <string> attributes_copy = new List<string>(considerable_attributes.ToArray());
            // Find best possible way to split these sets. For each attribute we will calculate the gain, and select the highest.
            string best_attr = "UNDETERMINED";
            double highest_gain = 0;
            foreach(string attr in attributes_copy)
            {
                agent.THINK("consider-attribute").set("attributes_left", attributes_copy.Count).finish();
                double my_gain = Calculator.gain(sets_todo, attr, this.target_attribute, this.possible_attribute_values[attr]);

                Dictionary<string, object> state = StateRecording.generateState("current_best_attribute", best_attr, "competing_attribute", attr, "current_best_gain", highest_gain, "competing_gain", my_gain, "parent_node", (parent_node != null) ? parent_node.identifier : "NULL");
                if (my_gain > highest_gain)
                {
                    agent.THINK("set-new-best-attribute").setState(state).finish();
                    best_attr = attr;
                    highest_gain = my_gain;
                }
                else
                {
                    agent.THINK("keep-old-attribute").setState(state).finish();
                }
            }
            agent.THINK("end-attribute-loop").set("attributes_left", 0).finish();

            if (highest_gain == 0)
            {
                // This set cannot be split further.
                // We have tried all attributes so we can't go further. The tree ends here my friend.
                // This happens when instances have all attributes the same except for the classifier.

                throw new Exception("This dataset contains instances with exactly the same attribute values but different classifiers, which this algorithm does not support.");

                // I previously made an implementation of the algorithm that adds a 'Best Guess leaf' to address this problem,
                // but this is not described as such in the algorithm description and has therefore been left out for the experimentation.


                agent.THINK("add-best-guess-leaf").set("best_attribute", best_attr).set("highest_gain", 0d).set("possible_attributes", attributes_copy.Count).finish();
                string classifier_value = SetHelper.mostCommonClassifier(sets_todo, target_attribute);
                Leaf leaf = tree.addBestGuessLeaf(parent_value_splitter, classifier_value, parent_node);
                tree.data_locations[leaf] = sets_todo;
                return tree;
            }

            // The best attribute to split this set is now saved in best_attr. Create a node for that.
            agent.THINK("add-node").set("best_attribute", best_attr).set("highest_gain", highest_gain).set("possible_attributes", attributes_copy.Count).finish();

            // Remove this attribute as a splitter for the dataset.
            attributes_copy.RemoveAt(considerable_attributes.IndexOf(best_attr));
            
            // Parent value splitter is to give a node an idea what it's parent splitted on. For decision rules this is needed information.
            Node new_node = tree.addNode(best_attr, parent_value_splitter, parent_node);

            // Create subsets for each possible value of the attribute we created a node for. 
            int values_left = this.possible_attribute_values[best_attr].Count;

            foreach (string value_splitter in this.possible_attribute_values[best_attr])
            {
                agent.THINK("subset-on-value").set("values_left", values_left).finish();
                List<DataInstance> subset = sets_todo.Where(A => A.getProperty(best_attr) == value_splitter).ToList();
                Dictionary<string, object> considering_state = StateRecording.generateState("split_attribute", best_attr, "split_value", value_splitter, "current_node", new_node.identifier, "parent_node", (parent_node != null) ? parent_node.identifier : "NULL");
                if (subset.Count == 0)
                {
                    // There are no more of this subset. We need to skip this iteration.
                    agent.THINK("ignore-value").setState(considering_state).finish();
                    continue;
                }
                if (SetHelper.hasUniformClassifier(subset, target_attribute))
                {
                    // This subset doesn't have to be split anymore. We can just add it to the node as a leaf. 
                    // Each leaf represents one decision rule. 
                    agent.THINK("add-leaf").setState(considering_state).finish();
                    string classifier_value = subset.First().getProperty(target_attribute);
                    Leaf leaf = tree.addLeaf(value_splitter, classifier_value, new_node);
                    tree.data_locations[leaf] = subset;
                } else
                {
                    // We still haven't resolved this set. We need to iterate upon it to split it again. 
                    agent.THINK("iterate-further").setState(considering_state).finish();
                    tree = this.iterate(tree, subset, attributes_copy, new_node, value_splitter);
                    // If we got here in the code then the set that was previously not all the same classifier has been resolved. We need to move up.
                }
                values_left -= 1;
            }
            agent.THINK("end-value-loop").set("values_left", values_left).finish();
            if (parent_node != null)
            {
                agent.THINK("return-tree-to-self").finish();
            }
            // We have succesfully split all examples on this attribute. Return the tree in its current state. 
            return tree;
        }
        
        public void calculateAttributePossibilities()
        {
            foreach (string attr in all_attributes)
            {
                // Make the list we will later add to the dictionary
                List<string> attribute_values = new List<string>();

                // Loop through all data instances to find the possible values.
                foreach(DataInstance instance in this.examples)
                {
                    string my_value = instance.getProperty(attr);
                    if (!attribute_values.Contains(my_value))
                    {
                        // A new possibility!
                        attribute_values.Add(my_value);

                    }
                }

                // Make the dictionary entry
                this.possible_attribute_values.Add(attr, attribute_values);
            }
        }

        private string subset_to_objectstring(List<DataInstance> l)
        {
            string ret = "{";
            for (int i = 0; i < l.Count; i++)
            {
                ret += l[i].identifier;
                if (i != l.Count - 1)
                {
                    ret += ",";
                }
            }
            ret += "}";
            return ret;
        }
        private string list_to_objectstring(List<string> str)
        {
            string ret = "{";
            for(int i = 0; i < str.Count; i++)
            {
                ret += str[i];
                if (i != str.Count - 1)
                {
                    ret += ",";
                }
            }
            ret += "}";
            return ret;
        }
    }
}
