using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class ID3 : Algorithm
    {
        private List<DataInstance> examples;
        private string target_attribute;
        private List<string> attributes;

        public DecisionTree train(List<DataInstance> examples, string target_attribute, List<string> attributes)
        {
            this.examples = examples;
            this.target_attribute = target_attribute;
            this.attributes = attributes;

            Console.WriteLine("Calculating set entropy...");
            Console.WriteLine(this.entropy(examples, "play").ToString());

            return new DecisionTree();
        }

        public double entropy(List<DataInstance> S, string target_attribute)
        {
            Dictionary<string, float> proportions = new Dictionary<string, float> ();

            foreach(DataInstance example in S)
            {
                string my_value = example.getProperty(target_attribute);

                if (!proportions.ContainsKey(my_value))
                {
                    Console.WriteLine($"Make entry in dictionary for {my_value}");
                    proportions[my_value] = 0;
                }
                Console.WriteLine($"Adding to {my_value}");
                proportions[my_value]++;
            }

            double result = 0;
            foreach(var item in proportions)
            {
                double proportion = (item.Value / S.Count());
                result -= (proportion * Math.Log(proportion, 2));
            }

            return result;
        }

        public double gain(List<DataInstance> S, string wanted_attribute, string targetAttribute)
        {
            double entropy = this.entropy(S, targetAttribute);

            return entropy;
        }
    }
}
