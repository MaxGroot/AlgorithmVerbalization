using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class C45Algorithm : Algorithm
    {
        private Dictionary<string, List<string>> possible_attribute_values = new Dictionary<string, List<string>>();

        public DecisionTree train(List<DataInstance> examples, string target_attribute, List<string> attributes, Agent runner)
        {
            Console.WriteLine("Calculating all attribute value possibilities..");
            foreach (string attr in attributes)
            {
                possible_attribute_values[attr] = Calculator.calculateAttributePossibilities(attr, examples);
            }
            Console.WriteLine("Let's go bois");
            return this.iterate(new DecisionTree(), examples, target_attribute, attributes, runner, null, null);
        }

        private DecisionTree iterate(DecisionTree tree, List<DataInstance> set, string target_attribute, List<string> attributes, Agent runner, Node parent, string last_split)
        {
            // Check termination criteria
            if (Calculator.subset_has_all_same_classifier(set, target_attribute))
            {
                // All same class: Create a leaf node for the decision tree.

                tree.addLeaf(last_split, set.First().getProperty(target_attribute), parent);
                return tree;
            }
            if (set.Count == 0)
            {
                // Set size 0: don't do anything
                return tree;
            }

            double highest_gain = 0.00;
            string a_best = "DAFAQ";
            foreach(string attr in attributes)
            {
                double my_gain = Calculator.splitInfo(set, attr, target_attribute, this.possible_attribute_values[attr]);
                if (my_gain > highest_gain)
                {
                    highest_gain = my_gain;
                    a_best = attr;
                }
            }
            Console.WriteLine($"{a_best} selected as best attribute");

            // Split on a best


            foreach(string attr in attributes)
            {
               // double gainratio = Calculator.splitInfo()
            }
            // Compute criteria for attributes
            double global_entropy = Calculator.entropy(set, target_attribute);

            // Choose best attribute

            // Create node for attribute

            // Split the set based on newly created node

            // For all sub data, call C4.5 to get a sub-tree

            // Attach sub-tree to the node of 4.

            // Return tree
            return tree;
        }
    }
}
