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

        private Dictionary<string, double> attribute_gains = new Dictionary<string, double>();

        private Dictionary<string, double> continuous_thresholds = new Dictionary<string, double>();

        private List<string> all_attribute_keys;

        private Dictionary<Leaf, List<DataInstance>> data_locations = new Dictionary<Leaf, List<DataInstance>>();
        
        public DecisionTree train(List<DataInstance> examples, string target_attribute, Dictionary<string, string> attributes, Agent runner)
        {
            foreach (string attr in attributes.Keys.ToList())
            {
                if (attributes[attr] == "nominal")
                {
                    possible_attribute_values[attr] = SetHelper.attributePossibilities(attr, examples);
                }else
                {
                    possible_attribute_values[attr] = null;
                }
            }
            all_attribute_keys = attributes.Keys.ToList();
            this.calculate_attribute_gains(examples, target_attribute, attributes, runner);

            DecisionTree full_tree = this.iterate(new DecisionTree(), examples, target_attribute, attributes, runner, null, null);

            runner.SNAPSHOT("pre-pruning", full_tree);
            Console.WriteLine("Initial tree constructed. Starting post-pruning. Creating queue of distinct nodes.");

            C45Pruner pruner = new C45Pruner();

            return pruner.prune(full_tree, target_attribute, data_locations);
        }

        private DecisionTree iterate(DecisionTree tree, List<DataInstance> set, string target_attribute, Dictionary<string, string> attributes, Agent runner, Node parent, string last_split)
        {

            // Select the best attribute to split on
            double highest_gain_ratio = -1;
            string best_split_attribute = "NOTFOUND";
            Boolean split_on_continuous = false;
            double threshold = 0;
            foreach (string attribute in attributes.Keys.ToList())
            {
                double my_gain_ratio = this.attribute_gains[attribute];
                if (my_gain_ratio > highest_gain_ratio)
                {
                    highest_gain_ratio = my_gain_ratio;
                    best_split_attribute = attribute;
                    split_on_continuous = (attributes[attribute] == "continuous");
                    if (split_on_continuous)
                    {
                        threshold = continuous_thresholds[attribute];
                    }
                }
            }

            // This attribute can now not be used anymore!
            Dictionary<string, string> attributes_for_further_iteration = AttributeHelper.CopyAttributeDictionary(attributes);
            attributes_for_further_iteration.Remove(best_split_attribute);

            // We now know the best splitting attribute and how to split it. We're gonna create the subsets now.
            Node newnode = null;

            Dictionary<string, List<DataInstance>> subsets = null;

            if (split_on_continuous)
            {
                newnode = tree.addContinuousNode(best_split_attribute, last_split, threshold, parent);

                subsets = SetHelper.subsetOnAttributeContinuous(set, best_split_attribute, threshold);
            }
            else
            {
                newnode = tree.addNode(best_split_attribute, last_split, parent);
                
                subsets = SetHelper.subsetOnAttributeNominal(set, best_split_attribute, possible_attribute_values[best_split_attribute]);
            }
            
            foreach(string subset_splitter in subsets.Keys)
            {
                List<DataInstance> subset = subsets[subset_splitter];

                if (subset.Count == 0)
                {
                    // There are no more of this subset. We need to skip this iteration.
                    continue;
                }
                if (SetHelper.hasUniformClassifier(subset, target_attribute))
                {
                    // This subset doesn't have to be split anymore. We can just add it to the node as a leaf. 
                    // Each leaf represents one decision rule. 
                    string classifier_value = subset.First().getProperty(target_attribute);

                    double certainty = 0;
                    foreach(DataInstance instance in subset)
                    {
                        certainty += instance.getWeight();
                    }
                    certainty /= (double)subset.Count;
                    
                    Leaf leaf = tree.addUncertainLeaf(subset_splitter, classifier_value, newnode, certainty);
                    this.data_locations[leaf] = subset;
                }
                else
                {
                    // We still haven't resolved this set. We need to iterate upon it to split it again. 
                    if (attributes_for_further_iteration.Count == 0)
                    {
                        string most_common_classifier = SetHelper.mostCommonClassifier(subset, target_attribute);

                        // Adjust for the uncertainty that comes with this prediction. 
                        double percentage_with_this_classifier = (double) subset.Where(A => A.getProperty(target_attribute) == most_common_classifier).ToList().Count / (double) subset.Count;
                        double certainty = 0;
                        foreach (DataInstance instance in subset)
                        {
                            certainty += instance.getWeight();
                        }
                        certainty /= (double)subset.Count;
                        
                        Leaf leaf = tree.addUncertainLeaf(subset_splitter, most_common_classifier, newnode, certainty * percentage_with_this_classifier);
                        this.data_locations[leaf] = subset;
                    }
                    else
                    {
                        this.iterate(tree, subset, target_attribute, attributes_for_further_iteration, runner, newnode, subset_splitter);
                    }
                    // If we got here in the code then the set that was previously not all the same classifier has been resolved. 
                    // Therefore we can let the foreach continue further!
                }
            }
            
            return tree;
        }

        private void calculate_attribute_gains(List<DataInstance> set, string target_attribute, Dictionary<string, string> attributes, Agent runner)
        {
            foreach (string attr in attributes.Keys.ToList())
            {
                if (attributes[attr] == "nominal")
                {
                    double my_ratio = Calculator.gainRatio(set, attr, target_attribute, possible_attribute_values[attr]);
                    this.attribute_gains[attr] = my_ratio;
                }

                if (attributes[attr] == "continuous")
                {
                    double[] split_and_ratio = Calculator.best_split_and_ratio_for_continuous(set, attr, target_attribute);
                    this.attribute_gains[attr] = split_and_ratio[1];
                    this.continuous_thresholds[attr] = split_and_ratio[0];
                }
            }
        }
    }
}
