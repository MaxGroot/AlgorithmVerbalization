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

        public DecisionTree train(List<DataInstance> examples, string target_attribute, Dictionary<string, string> attributes, Agent runner)
        {
            Console.WriteLine("Calculating all attribute value possibilities..");
            foreach (string attr in attributes.Keys.ToList())
            {
                if (attributes[attr] == "nominal")
                {
                    possible_attribute_values[attr] = Calculator.calculateAttributePossibilities(attr, examples);
                }else
                {
                    possible_attribute_values[attr] = null;
                }
            }
            return this.iterate(new DecisionTree(), examples, target_attribute, attributes, runner, null, null);
        }

        private DecisionTree iterate(DecisionTree tree, List<DataInstance> set, string target_attribute, Dictionary<string, string> attributes, Agent runner, Node parent, string last_split)
        {
            string best_split_attribute = "";
            Boolean split_on_continuous = false;
            double threshold = 0;
            double highest_gain_ratio = -1;

            foreach(string attr in attributes.Keys.ToList())
            {
                if (attributes[attr] == "nominal")
                {
                    double my_ratio = Calculator.gainRatio(set, attr, target_attribute, possible_attribute_values[attr]);
                    if (my_ratio > highest_gain_ratio)
                    {
                         highest_gain_ratio = my_ratio;
                         best_split_attribute = attr;
                         split_on_continuous = false;
                    }
                }

                if (attributes[attr] == "continuous")
                {
                    double[] split_and_ratio = Calculator.best_split_and_ratio_for_continuous(set, attr, target_attribute);
                    Console.WriteLine($" Best split on {split_and_ratio[0]} with ratio {split_and_ratio[1]}");

                    if (split_and_ratio[1] > highest_gain_ratio)
                    {
                        highest_gain_ratio = split_and_ratio[1];
                        threshold = split_and_ratio[0];
                        split_on_continuous = true;
                        best_split_attribute = attr;
                    }
                }
            }
            // We now know the best splitting attribute and how to split it. We're gonna create the subsets now.
            Node newnode = null;

            Dictionary<string, List<DataInstance>> subsets = null;

            if (split_on_continuous)
            {
                newnode = tree.addContinuousNode(best_split_attribute, last_split, threshold, parent);
                Console.WriteLine($" C NODE: {best_split_attribute} ON {threshold}");

                subsets = Calculator.subsetOnAttributeContinuous(set, best_split_attribute, threshold);
            }
            else
            {
                newnode = tree.addNode(best_split_attribute, last_split, parent);

                Console.WriteLine($" N NODE: {best_split_attribute}");

                subsets = Calculator.subsetOnAttributeNominal(set, best_split_attribute, possible_attribute_values[best_split_attribute]);
            }

            Console.WriteLine("Subsets made. Iterating on subsets. ");
            
            foreach(string subset_splitter in subsets.Keys)
            {
                List<DataInstance> subset = subsets[subset_splitter];

                if (subset.Count == 0)
                {
                    // There are no more of this subset. We need to skip this iteration.
                    continue;
                }
                if (Calculator.subset_has_all_same_classifier(subset, target_attribute))
                {
                    // This subset doesn't have to be split anymore. We can just add it to the node as a leaf. 
                    // Each leaf represents one decision rule. 
                    Console.WriteLine("Making leaf");
                    string classifier_value = subset.First().getProperty(target_attribute);
                    Leaf leaf = tree.addLeaf(subset_splitter, classifier_value, newnode);
                }
                else
                {
                    // We still haven't resolved this set. We need to iterate upon it to split it again. 

                    this.iterate(tree, subset, target_attribute, attributes, runner, newnode, subset_splitter);

                    // If we got here in the code then the set that was previously not all the same classifier has been resolved. We need to move up.
                }
            }
            Console.WriteLine("We looped through");

            return tree;
        }
    }
}
