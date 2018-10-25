using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    static class Calculator
    {
        public static double entropy(List<DataInstance> S, string attribute_key)
        {
            // Initialize a dictionary that will count for each value of the target attribute how many times it occures within a set. 
            Dictionary<string, float> proportions = new Dictionary<string, float>();

            
            foreach (DataInstance example in S)
            {
                string my_value = example.getProperty(attribute_key);
                
                if (!proportions.ContainsKey(my_value))
                {
                    proportions[my_value] = 0;
                }
                // Increase the counter of this value of the target attribute.
                proportions[my_value]++;
            }

            double result = 0;

            // Loop through the dictionary of possible values of this attribute
            foreach (var item in proportions)
            {
                double proportion = (item.Value / S.Count());

                // Make this value possibility's contribution to the entropy
                result -= (proportion * Math.Log(proportion, 2));
            }

            // After the loop, we have the correct entropy.
            return result;
        }


        public static double gain(List<DataInstance> S, string wanted_attribute, string targetAttribute,  List <string> possible_values)
        {
            // First find the entropy of the target attribute over the given set.
            double result = entropy(S, targetAttribute);
            
            // Loop through possible values of the wanted attribute
            foreach (string value in possible_values)
            {
                // Create a subset of data instances that only contain this value.
                List<DataInstance> set_filtered = S.Where(A => A.getProperty(wanted_attribute) == value).ToList();


                // Calculate the proportion of this value in the set towards the size of the entire set.
                double proportion = ((double)set_filtered.Count()) / ((double)S.Count());

                // Calculate the entropy of the target attribute over this subset.
                double subset_entropy = entropy(set_filtered, targetAttribute);

                // Make this value possibility's contribution towards the information gain.
                result -= proportion * subset_entropy;
            }

            return result;
        }
    }
}
