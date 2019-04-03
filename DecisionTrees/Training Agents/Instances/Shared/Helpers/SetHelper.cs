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
                if (value_splitter != null)
                {
                    List<DataInstance> subset = set.Where(A => A.getProperty(attribute) == value_splitter).ToList();
                    subset_collection.Add(value_splitter, subset);
                }
            }
            if (possible_attribute_values.Contains(null))
            {
                List<DataInstance> subset_of_original_missings = set.Where(A => A.getProperty(attribute) == null).ToList();
                // There were instances with null for this attribute. We need to distribute them accordingly.
                foreach(string value_splitter in possible_attribute_values)
                {
                    if (value_splitter != null)
                    {
                        double weight = (double)subset_collection[value_splitter].Count / (double) set.Count;

                        // Copy the original subset of missings to a new subset that has been weighted and add it to the other instances
                        // in the subset
                        List<DataInstance> weighted_copies = copyWithWeight(subset_of_original_missings, weight);
                        subset_collection[value_splitter].AddRange(weighted_copies); 
                    }
                }
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

        public static double missingDataFraction(List<DataInstance> S, string wanted_attribute)
        {
            int missingInstances = 0;
            foreach (DataInstance a in S)
            {
                if (a.getProperty(wanted_attribute) == null)
                {
                    missingInstances++;
                }
            }
            return (double)missingInstances / (double) S.Count;
        }

        public static List<DataInstance> copyWithWeight(List<DataInstance> set, double weight)
        {
            List<DataInstance> output = new List<DataInstance>();
            foreach(DataInstance old in set)
            {
                DataInstance instance = DataInstance.copy($"weighted-copy-of-{old.identifier}", old);
                instance.setWeight(weight);
                output.Add(instance);
            }
            return output;
        }
    }
}
