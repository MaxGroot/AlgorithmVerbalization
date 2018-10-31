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
        private List<string> attributes;
        private Dictionary<string, List<string>> possible_attribute_values = new Dictionary<string, List<string>> ();
 
        public DecisionTree train(List<DataInstance> examples, string target_attribute, List<string> attributes)
        {
            this.examples = examples;
            this.target_attribute = target_attribute;
            this.attributes = attributes;

            // First we need to know for each attribute which possible values it can hold.
            Console.WriteLine("Calculating possible values of all attributes...");
            this.calculateAttributePossibilities();
        
            DecisionTree tree = new DecisionTree();
            
            // Start the iteration process on the entire set.
            return this.iterate(tree, this.examples);
        }

        public DecisionTree iterate(DecisionTree tree, List<DataInstance> sets_todo, string parent_value_splitter = null)
        {
            if (this.attributes.Count == 0)
            {
                // We have tried all attributes so we can't go further. The tree ends here my friend.
                Console.WriteLine($"Could not find suitable splitter for these {sets_todo.Count} examples.");
                return tree;
            }
            // Find best possible way to split these sets. For each attribute we will calculate the gain, and slect the highest.
            string best_attr = "";
            double highest_gain = 0;
            foreach(string attr in this.attributes)
            {
                double my_gain = Calculator.gain(sets_todo, attr, this.target_attribute, this.possible_attribute_values[attr]);
                if (my_gain > highest_gain)
                {
                    best_attr = attr;
                    highest_gain = my_gain;
                }
            }

            // The best attribute to split this set is now saved in best_attr. Create a node for that.
            // Remove this attribute as a splitter for the dataset.
            this.attributes.RemoveAt(attributes.IndexOf(best_attr));
            
            // Parent value splitter is to give a node an idea what it's parent splitted on. For decision rules this is needed information.
            tree.addNode(best_attr, parent_value_splitter);
            

            // Create subsets for each possible value of the attribute we created a node for. 
            foreach (string value_splitter in this.possible_attribute_values[best_attr])
            {
                List<DataInstance> subset = sets_todo.Where(A => A.getProperty(best_attr) == value_splitter).ToList();
                if (subset.Count == 0)
                {
                    // There are no more of this subset. We need to skip this iteration.
                    continue;
                }
                if (Calculator.subset_has_all_same_classifier(subset, target_attribute))
                {
                    // This subset doesn't have to be split anymore. We can just add it to the node as a leaf. 
                    // Each leaf represents one decision rule. 
                    
                    Leaf leaf = tree.addLeaf(value_splitter, subset.First().getProperty(target_attribute));
                    Console.WriteLine(leaf.myRule(target_attribute));
                } else
                {
                    // We still haven't resolved this set. We need to iterate upon it to split it again. 
                    this.iterate(tree, subset, value_splitter);

                    // If we got here in the code then the set that was previously not all the same classifier has been resolved. We need to move up.
                    tree.moveSelectionUp();
                }
            }

            // We have succesfully split all examples on this attribute. Return the tree in its current state. 
            return tree;
        }
        
        public void calculateAttributePossibilities()
        {
            foreach (string attr in attributes)
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
