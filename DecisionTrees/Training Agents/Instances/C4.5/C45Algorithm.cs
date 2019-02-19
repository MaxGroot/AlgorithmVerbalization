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
            foreach(string attr in attributes.Keys.ToList())
            {
                Console.WriteLine($"Calculating stuff for {attr}..");
                if (attributes[attr] == "nominal")
                {
                   // Console.WriteLine($"Gain: {Calculator.gain(set, attr, target_attribute, possible_attribute_values[attr])}");
                   // Console.WriteLine($"Gainratio: {Calculator.gainRatio(set, attr, target_attribute, possible_attribute_values[attr])}");
                }
                if (attributes[attr] == "continuous")
                {
                    Console.WriteLine("Continuous variable..");
                    double[] split_and_ratio = Calculator.best_split_and_ratio_for_continuous(set, attr, target_attribute);
                    Console.WriteLine($" Best split on {split_and_ratio[0]} with ratio {split_and_ratio[1]}");
                }
            }
            throw new NotImplementedException("HOI");
            return tree;
        }
    }
}
