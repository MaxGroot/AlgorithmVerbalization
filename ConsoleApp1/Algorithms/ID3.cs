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

            Console.WriteLine("Calculating possible values of all attributes...");
            this.calculateAttributePossibilities();
        
            DecisionTree tree = new DecisionTree();
            
            return this.iterate(tree, this.examples);
        }

        public DecisionTree iterate(DecisionTree tree, List<DataInstance> sets_todo, string parent_value_splitter = null)
        {
            // Find best possible way to split these sets.
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
            tree.addNode(best_attr, parent_value_splitter);
            
            // Create subsets for each possible value of the best attribute
            foreach (string value_splitter in this.possible_attribute_values[best_attr])
            {
                List<DataInstance> subset = sets_todo.Where(A => A.getProperty(best_attr) == value_splitter).ToList();
                if (this.subset_has_all_same_classifier(subset, target_attribute))
                {
                    // This subset doesn't have to be split anymore. We can just add it to the node.
                    Leaf leaf = tree.addLeaf(value_splitter, subset.First().getProperty(target_attribute));
                    Console.WriteLine(leaf.myRule(target_attribute));
                } else
                {
                    // We still haven't resolved this set. We need to iterate upon it.
                    this.iterate(tree, subset, value_splitter);

                    // If we got here in the code then the above set has been resolved. We need to move up.
                    tree.moveSelectionUp();
                }  
            }

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

        public bool subset_has_all_same_classifier(List<DataInstance> S, string target_attribute)
        {
            string classifier = S.First().getProperty(target_attribute);

            foreach(DataInstance instance in S)
            {
                if (instance.getProperty(target_attribute) != classifier)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
