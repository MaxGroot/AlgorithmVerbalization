using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class SetHelper
    {
        public static Dictionary<string, List<DataInstance>> subsetOnAttributeNominal(List<DataInstance> set, string attribute, List<string> possible_attribute_values)
        {
            Dictionary<string, List<DataInstance>> subset_collection = new Dictionary<string, List<DataInstance>>();

            foreach (string value_splitter in possible_attribute_values)
            {
                List<DataInstance> subset = set.Where(A => A.getProperty(attribute) == value_splitter).ToList();
                subset_collection.Add(value_splitter, subset);
            }
            return subset_collection;
        }

        public static Dictionary<string, List<DataInstance>> subsetOnAttributeContinuous(List<DataInstance> set, string attribute, double threshold)
        {
            Dictionary<string, List<DataInstance>> subset_collection = new Dictionary<string, List<DataInstance>>();

            List<DataInstance> less_than_equal = set.Where(A => A.getPropertyAsDouble(attribute) <= threshold).ToList();

            List<DataInstance> above = set.Where(A => A.getPropertyAsDouble(attribute) > threshold).ToList();

            subset_collection.Add("<=", less_than_equal);
            subset_collection.Add(">", above);
            return subset_collection;
        }

        public static bool hasUniformClassifier(List<DataInstance> S, string target_attribute)
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

        public static string mostCommonClassifier(List<DataInstance> S, string target_attribute)
        {
            // Initialize Dictionary
            Dictionary<String, int> value_counter = new Dictionary<string, int>();

            // Set classifier frequency in dictionary
            foreach (DataInstance instance in S)
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

        public static List<string> attributePossibilities(string attr_name, List<DataInstance> Set)
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

    }
}
