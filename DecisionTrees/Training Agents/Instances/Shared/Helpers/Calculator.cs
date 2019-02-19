using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    static class Calculator
    {
        private static List<string> alphabet()
        {
            return new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }
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

        public static bool subset_has_all_same_classifier(List<DataInstance> S, string target_attribute)
        {
            // First find any classifier that we will check for on the entire set
            string classifier = S.First().getProperty(target_attribute);

            foreach (DataInstance instance in S)
            {
                if (instance.getProperty(target_attribute) != classifier)
                {
                    // We found another classifier than the one we previously had. That means this subset has different classifiers. 
                    return false;
                }
            }

            // If we are here then the subset has the same classifier.
            return true;
        }

        public static string subset_most_common_classifier(List<DataInstance> S, string target_attribute)
        {
            // Initialize Dictionary
            Dictionary<String, int> value_counter = new Dictionary<string, int>();
            
            // Set classifier frequency in dictionary
            foreach(DataInstance instance in S)
            {
                string my_classifier = instance.getProperty(target_attribute);
                if (!value_counter.ContainsKey(my_classifier))
                {
                    value_counter[my_classifier] = 0;
                }

                value_counter[my_classifier]++;
            }
            // Return classifier value that occurs most in the given set.
            return value_counter.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        }

        public static double intrinsicValue(List<DataInstance> S, string wanted_attribute, List<string> possible_values)
        {
            double intrinsic = 0;
            foreach(string value in possible_values)
            {
                // Create a subset of data instances that only contain this value.
                List<DataInstance> set_filtered = S.Where(A => A.getProperty(wanted_attribute) == value).ToList();
                
                // Calculate the proportion of this value in the set towards the size of the entire set.
                double proportion = ((double)set_filtered.Count()) / ((double)S.Count());

                // Multiply the proportion with log base 2 of itself. 
                proportion *= Math.Log(proportion, 2);

                // Add to intrinsic value
                intrinsic += proportion;

            }
            // Return the negative of the sum.
            return -intrinsic;
        }

        public static double splitInfo(List<DataInstance> S, string wanted_attribute, string targetAttribute, List<string> possible_values)
        {
            return gain(S, wanted_attribute, targetAttribute, possible_values) / intrinsicValue(S, wanted_attribute, possible_values);
        }

        public static List<string> calculateAttributePossibilities(string attr_name, List<DataInstance> Set)
        {
            // Make the list we will later add to the dictionary
            List<string> attribute_values = new List<string>();
            // Loop through all data instances to find the possible values.
            foreach (DataInstance instance in Set)
            {
                string my_value = instance.getProperty(attr_name);
                if (!attribute_values.Contains(my_value))
                {
                    // A new possibility!
                    attribute_values.Add(my_value);

                }
            }
            return attribute_values;
        }

        public static string generateElementId(char character, int counter)
        {
            int second_letter_count = counter % 26;
            int first_letter_count = (int)counter / 26;

            return character.ToString() + "-"  + alphabet()[first_letter_count] + alphabet()[second_letter_count];
        }
    }
}
