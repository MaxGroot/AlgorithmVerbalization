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
                Console.WriteLine($"Calculating stuff for {attr}..");
                if (attributes[attr] == "nominal")
                {
                    double my_ratio = Calculator.gainRatio(set, attr, target_attribute, possible_attribute_values[attr]);
                    Console.WriteLine($"Ratio: {my_ratio}");
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
            Console.WriteLine($"{best_split_attribute} was selected. Making node.");
            Node newnode = null;

            List<List<DataInstance>> subsets = null;
            if (split_on_continuous)
            {
                newnode = tree.addContinuousNode(best_split_attribute, last_split, threshold, parent);
                List<DataInstance> less_then_equal = set.Where(A => A.getPropertyAsDouble(best_split_attribute) <= threshold).ToList();
                List<DataInstance> above = set.Where(A => A.getPropertyAsDouble(best_split_attribute) > threshold).ToList();

                subsets = Calculator.subsetOnAttributeContinuous(set, best_split_attribute, threshold);

            } else
            {
                newnode = tree.addNode(best_split_attribute, last_split, parent);

                subsets = Calculator.subsetOnAttributeNominal(set, best_split_attribute, possible_attribute_values[best_split_attribute]);
            }

            Console.WriteLine("Subsets made.");
            throw new NotImplementedException("HALLO");

            // We got the node, now iterate on the subsets.
            Console.WriteLine("");

            return tree;
        }
    }
}
