using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using System.Net.Http;

namespace DecisionTrees
{
    static class Calculator
    {
        private static readonly HttpClient client = new HttpClient();

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

            // Adjust for missing data
            double missingFraction = SetHelper.missingDataFraction(S, wanted_attribute);
            
            return (1 - missingFraction) * gain(S, wanted_attribute, targetAttribute, possible_values) / splitInfo(S, wanted_attribute, possible_values);
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
            // Adjust for missing data
            double missingFraction = SetHelper.missingDataFraction(S, wanted_attribute);
            best_split_gain_ratio = (1 - missingFraction) * best_split_gain_ratio;
            return new double[] { best_split, best_split_gain_ratio};
        }
     
        public static double upperBound(double f, double N, double z)
        {
            double fOverN = (f / N);
            double fSquaredOverN = ((f * f) / N);
            double zSquaredOver4Nsquared = ((z * z) / (4 * N * N));

            double zSquaredOver2N = ((z * z) / (2 * N));
            double onePlusZsquaredOverN = 1 + ((z * z) / N);
            
            double sqrt = Math.Sqrt(fOverN - fSquaredOverN + zSquaredOver4Nsquared);

            return (f + zSquaredOver2N + (z * sqrt)) / onePlusZsquaredOverN;
        }

        public static double upperBoundGood(int successes, int sampleSize, int confidence)
        {

            var values = new Dictionary<string, string>
            {
               { "func", $"BinomialHigh({successes},{sampleSize},{confidence})" },
            };

            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync("https://www.medcalc.org/inc/getmedcalcvalue.php", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;
                try
                {
                    double upperBound = Double.Parse(responseString);
                    return upperBound;
                }catch(FormatException e)
                {
                    throw new Exception($"Could not convert {responseString} to a double..");
                }
            }else
            {
                throw new Exception("Http request not succesful.");
            }         
        }
    }
}
