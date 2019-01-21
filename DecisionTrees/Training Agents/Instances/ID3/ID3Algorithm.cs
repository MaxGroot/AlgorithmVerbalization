using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ID3 : Algorithm
    {
        private List<DataInstance> examples;
        private string target_attribute;
        private List<string> all_attributes;
        private Dictionary<string, List<string>> possible_attribute_values = new Dictionary<string, List<string>> ();
        private Agent runner;
        
        // Make System State Descriptors
        private SystemStateDescriptor calculate_gain = new SystemStateDescriptor("Calculate Attribute Gain", new List<string> { "attr", "my_gain", "highest_gain" });
        private SystemStateDescriptor best_attribute = new SystemStateDescriptor("Select best attribute", new List<string> { "my_gain","highest_gain"});
        private SystemStateDescriptor subset_size = new SystemStateDescriptor("Receive Subset For Iteration", new List<string> { "subset_size" });

        public DecisionTree train(List<DataInstance> examples, string target_attribute, List<string> attributes, Agent runner)
        {
            this.examples = examples;
            this.target_attribute = target_attribute;
            this.all_attributes = attributes;
            this.runner = runner;

            // Prepare our runner with the right way to describe system state.
            runner.prepare_system_state(new List<SystemStateDescriptor>() {
                calculate_gain,
                best_attribute,
                subset_size,
            });

            // First we need to know for each attribute which possible values it can hold.
            this.calculateAttributePossibilities();
        
            DecisionTree tree = new DecisionTree();

            // Start the iteration process on the entire set.
            tree = this.iterate(tree, this.examples, 1, attributes, null, null);
            return tree;
        }

        public DecisionTree iterate(DecisionTree tree, List<DataInstance> sets_todo, int level, List<string> considerable_attributes, Node parent_node, string parent_value_splitter)
        {
            List <string> attributes_copy = new List<string>(considerable_attributes.ToArray());
            // Find best possible way to split these sets. For each attribute we will calculate the gain, and select the highest.
            string best_attr = "UNDETERMINED";
            double highest_gain = 0;
            this.runner.THINK(new SystemState(sets_todo.Count.ToString()).setDescriptor(subset_size));
            foreach(string attr in attributes_copy)
            {
                double my_gain = Calculator.gain(sets_todo, attr, this.target_attribute, this.possible_attribute_values[attr]);
                this.runner.THINK( new SystemState(attr, my_gain, highest_gain).setDescriptor(calculate_gain));
                if (my_gain > highest_gain)
                {
                    best_attr = attr;
                    highest_gain = my_gain;
                    this.runner.THINK(new SystemState(my_gain, highest_gain).setDescriptor(best_attribute));
                }
            }
            if (highest_gain == 0)
            {
                // This set cannot be split further.
                // We have tried all attributes so we can't go further. The tree ends here my friend.
                // This happens when instances have all attributes the same except for the classifier.
                this.runner.ACT(new DecisionAddBestGuessLeaf()
                       .setProof(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", parent_value_splitter } }).
                       setAppliedAction(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", parent_value_splitter } }));
                string classifier_value = Calculator.subset_most_common_classifier(sets_todo, target_attribute);
                tree.addBestGuessLeaf(parent_value_splitter, classifier_value, parent_node);
                return tree;
            }

            // The best attribute to split this set is now saved in best_attr. Create a node for that.
            // Remove this attribute as a splitter for the dataset.
            this.runner.ACT(new DecisionRemoveAttributeFromConsideration()
                        .setProof(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_gain", highest_gain.ToString() } }).
                        setAppliedAction(new Dictionary<string, string>() { { "attribute_name", best_attr } }));
            attributes_copy.RemoveAt(considerable_attributes.IndexOf(best_attr));

            // Parent value splitter is to give a node an idea what it's parent splitted on. For decision rules this is needed information.
            this.runner.ACT(new DecisionAddNode()
                        .setProof(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_gain", highest_gain.ToString() } }).
                        setAppliedAction(new Dictionary<string, string>() { { "attribute_name", best_attr } }));
            Node new_node = tree.addNode(best_attr, parent_value_splitter, parent_node);
            
            // Create subsets for each possible value of the attribute we created a node for. 
            foreach (string value_splitter in this.possible_attribute_values[best_attr])
            {
                List<DataInstance> subset = sets_todo.Where(A => A.getProperty(best_attr) == value_splitter).ToList();
                if (subset.Count == 0)
                {
                    // There are no more of this subset. We need to skip this iteration.
                    this.runner.ACT(new DecisionSkipAttributeValueCombination()
                        .setProof(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", value_splitter } }).
                        setAppliedAction(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", value_splitter } }));
                    continue;
                }
                if (Calculator.subset_has_all_same_classifier(subset, target_attribute))
                {
                    // This subset doesn't have to be split anymore. We can just add it to the node as a leaf. 
                    // Each leaf represents one decision rule. 

                    string classifier_value = subset.First().getProperty(target_attribute);
                    this.runner.ACT(new DecisionAddLeaf()
                        .setProof(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", value_splitter }, { "classifier_value", classifier_value } }).
                        setAppliedAction(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", value_splitter } }));
                    Leaf leaf = tree.addLeaf(value_splitter, classifier_value, new_node);
                } else
                {
                    // We still haven't resolved this set. We need to iterate upon it to split it again. 
                    this.runner.ACT(new DecisionIterateFurther()
                        .setProof(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", value_splitter } }).
                        setAppliedAction(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", value_splitter } }));

                    this.iterate(tree, subset, level+1, attributes_copy, new_node, value_splitter);

                    // If we got here in the code then the set that was previously not all the same classifier has been resolved. We need to move up.
                    this.runner.ACT(new DecisionMoveupToAttribute()
                        .setProof(new Dictionary<string, string>() { { "attribute_name", best_attr }, { "attribute_value", value_splitter } }).
                        setAppliedAction(new Dictionary<string, string>() { { "parent_attribute", parent_value_splitter } }));
                }
            }

            // We have succesfully split all examples on this attribute. Return the tree in its current state. 
           this.runner.ACT(new DecisionMoveUpToValue()
                        .setProof(new Dictionary<string, string>() { { "attribute_name", best_attr } }).
                        setAppliedAction(new Dictionary<string, string>() { { "parent_value", parent_value_splitter } }));
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
    }
}
