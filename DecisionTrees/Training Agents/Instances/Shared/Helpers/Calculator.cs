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

        public static double gain(List<DataInstance> S, string wanted_attribute, string targetAttribute, List<string> possible_values)
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

        public static double splitInfo(List<DataInstance> S, string wanted_attribute, List<string> possible_values)
        {
            double splitinfo = 0;
            foreach (string value in possible_values)
            {
                // Create a subset of data instances that only contain this value.
                List<DataInstance> set_filtered = S.Where(A => A.getProperty(wanted_attribute) == value).ToList();

                // Calculate the proportion of this value in the set towards the size of the entire set.
                double proportion = ((double)set_filtered.Count()) / ((double)S.Count());

                // Multiply the proportion with log base 2 of itself. 
                proportion *= Math.Log(proportion, 2);

                // Add to intrinsic value
                splitinfo += proportion;

            }
            // Return the negative of the sum.
            return -splitinfo;
        }

        public static double gainRatio(List<DataInstance> S, string wanted_attribute, string targetAttribute, List<string> possible_values)
        {
            return gain(S, wanted_attribute, targetAttribute, possible_values) / splitInfo(S, wanted_attribute, possible_values);
        }

        public static double[] best_split_and_ratio_for_continuous(List<DataInstance> S, string wanted_attribute, string target_attribute)
        {
            double total_set_entropy = entropy(S, target_attribute);

            // Sort by the wanted attribute
            List<DataInstance> s_sorted = S.OrderBy(o => o.getProperty(wanted_attribute)).ToList();
            List<double> possible_values = new List<double>();

            // Add posisble attribute values based on instances supplied.
            foreach (DataInstance instance in s_sorted)
            {
                double my_value = instance.getPropertyAsDouble(wanted_attribute);
                if (!possible_values.Contains(my_value))
                {
                    possible_values.Add(my_value);
                }
            }

            // Loop through possible splits and calculate their gain ratios

            double best_split = 0;
            double best_split_gain = -1;
            double best_split_gain_ratio = -1;
            bool found_better_than_nothing = false;
            foreach (double binary_split in possible_values)
            {

                // Create subsets below or equal, and above the wanted attribute's current binary split. 
                List<DataInstance> s_below_or_equal = S.Where(o => o.getPropertyAsDouble(wanted_attribute) <= binary_split).ToList();
                List<DataInstance> s_above = S.Where(o => o.getPropertyAsDouble(wanted_attribute) > binary_split).ToList();

                double entropy_below_or_equal = entropy(s_below_or_equal, target_attribute);
                double entropy_above = entropy(s_above, target_attribute);

                double proportion_below_or_equal = ((double)s_below_or_equal.Count()) / ((double)S.Count());
                double proportion_above = ((double)s_above.Count()) / ((double)S.Count());

                // Calculare gain of splitting on this binary split
                double gain_on_this_split = total_set_entropy - (proportion_below_or_equal * entropy_below_or_equal) - (proportion_above * entropy_above);
                double splitinfo_on_this_split = -(proportion_below_or_equal * Math.Log(proportion_below_or_equal, 2)) - (proportion_above * Math.Log(proportion_above, 2));
                double gain_ratio_on_this_split = gain_on_this_split / splitinfo_on_this_split;

                // Finally all calculations are done! Lets find out if this one is the best one yet.
                if (gain_on_this_split > best_split_gain)
                {
                    found_better_than_nothing = true;
                    best_split_gain = gain_on_this_split;
                    best_split_gain_ratio = gain_ratio_on_this_split;
                    best_split = binary_split;
                }
            }
            if (!found_better_than_nothing)
            {
                Console.WriteLine($"No gain ratio could be found for this attribute {wanted_attribute}");
            }
            return new double[] { best_split, best_split_gain_ratio };
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

            return character.ToString() + "-" + alphabet()[first_letter_count] + alphabet()[second_letter_count];
        }

        public static List<List<DataInstance>> subsetOnAttributeNominal(List<DataInstance> set, string attribute, List<string> possible_attribute_values)
        {
            List<List<DataInstance>> list_of_subsets = new List<List<DataInstance>>();
            foreach (string value_splitter in possible_attribute_values)
            {
                List<DataInstance> subset = set.Where(A => A.getProperty(attribute) == value_splitter).ToList();
                Console.WriteLine($"Subset on {value_splitter} : {subset.Count} instances out of {set.Count}.");
                list_of_subsets.Add(subset);
            }
            return list_of_subsets;
        }


        public static List<List<DataInstance>> subsetOnAttributeContinuous(List<DataInstance> set, string attribute, double threshold)
        {
            List<List<DataInstance>> list_of_subsets = new List<List<DataInstance>>();
            List<DataInstance> less_than_equal = set.Where(A => A.getPropertyAsDouble(attribute) <= threshold).ToList();
            Console.WriteLine($" <= {threshold} : {less_than_equal.Count} / {set.Count}");

            List<DataInstance> above = set.Where(A => A.getPropertyAsDouble(attribute) > threshold).ToList();
            Console.WriteLine($" > {threshold} : {above.Count} / {set.Count}");

            list_of_subsets.Add(less_than_equal);
            list_of_subsets.Add(above);
            return list_of_subsets;
        }
    }
}
