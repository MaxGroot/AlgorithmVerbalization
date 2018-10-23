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
        private Dictionary<string, List<string>> possible_attribute_values = new Dictionary<string, List<string>> ();

        public DecisionTree train(List<DataInstance> examples, string target_attribute, List<string> attributes)
        {
            this.examples = examples;
            this.target_attribute = target_attribute;
            this.attributes = attributes;

            Console.WriteLine("Calculating possible values of all attributes...");
            this.calculateAttributePossibilities();

            Console.WriteLine("Calculating set entropy...");
            Console.WriteLine(this.entropy(examples, "play").ToString());

            
            Console.WriteLine("Calculating attribute gains...");
            foreach(string attr in attributes)
            {
                Console.WriteLine($"{attr} : {this.gain(examples, attr, target_attribute)}");
            }
            
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
                    proportions[my_value] = 0;
                }
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
            double result = this.entropy(S, targetAttribute);
            

            List<string> possible_values = this.possible_attribute_values[wanted_attribute];

            foreach(string value in possible_values)
            {
                // Create a subset of data instances that only contain this value.
                List<DataInstance> set_filtered = S.Where(A => A.getProperty(wanted_attribute) == value).ToList();
                

              
                double proportion = ( (double) set_filtered.Count() ) / ( (double) S.Count() );
                double subset_entropy = this.entropy(set_filtered, targetAttribute);
                
                result -= proportion * subset_entropy;
            }

            return result;
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
