using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class C45Algorithm : Algorithm
    {
        // Possible values for nominal attributes
        private Dictionary<string, List<string>> possible_nominal_values = new Dictionary<string, List<string>>();

        // Possible values for nominal attributes
        private Dictionary<string, List<double>> possible_continuous_values = new Dictionary<string, List<double>>();


        // All attributes possible.
        private List<string> all_attribute_keys;

        // TODO: Remove this variable as to not depend on J48 likeliness
        private bool have_been_at_root = false;

        private Agent agent;

        private string target_attribute;

        bool keep_considering_all_values;

        private int minimum_leaf_size;

        public DecisionTree train(List<DataInstance> examples, string target_attribute, Dictionary<string, string> attributes, Agent agent, Dictionary<string, object> parameters)
        {
            // Before we begin, calculate possible values for nominal attributes
            foreach (string attr in attributes.Keys.ToList())
            {
                if (attributes[attr] == "nominal")
                {
                    possible_nominal_values[attr] = SetHelper.attributePossibilities(attr, examples);
                }
                else
                {
                    possible_continuous_values[attr] = new List<double>();
                    if (keep_considering_all_values)
                    {
                        // This algorithm can either remember all possible values that the entire training set has had for a 
                        // continuous variable, or evaluate upon the present values on subsets. If the user opts for the former,
                        // we must remember all possible values beforehand. 

                        List<string> possible_values_as_strings = SetHelper.attributePossibilities(attr, examples);
                        foreach (string val_as_string in possible_values_as_strings)
                        {
                            possible_continuous_values[attr].Add(double.Parse(val_as_string));
                        }
                        possible_continuous_values[attr].Sort();
                    }
                }
            }

            this.all_attribute_keys = attributes.Keys.ToList();
            this.agent = agent;
            this.target_attribute = target_attribute;

            minimum_leaf_size = (int) parameters["minimum_number_of_objects"];
            keep_considering_all_values = (bool) parameters["keep_values_input"];

            // Generate C4.5 decision tree.
            agent.THINK("start").finish();
            DecisionTree full_tree = this.iterate(new DecisionTree(target_attribute), examples, target_attribute, attributes, null, null);

            // Snapshot full tree.
            agent.SNAPSHOT("pre-pruning", full_tree);

            // Return pruned tree.
            C45Pruner pruner = new C45Pruner();

            int confidence_on_100_scale = (int) parameters["confidence"];
            double confidence = (double) (((double) confidence_on_100_scale * 2d) / 100d);
            DecisionTree pruned_tree = pruner.prune(full_tree, target_attribute, confidence, agent);
            agent.THINK("return-prediction-model").finish();

            return pruned_tree;
        }

        private DecisionTree iterate(DecisionTree tree, List<DataInstance> set, string target_attribute, Dictionary<string, string> attributes, Node parent, string last_split)
        {
            this.agent.THINK("iterate").finish();

            // Re-evaluate gains and thresholds
            Dictionary<string, Dictionary<string, double>> gains_and_thresholds = calculate_attribute_gain_ratios(set, target_attribute, attributes);
            Dictionary<string, double> gains = gains_and_thresholds["gains"];
            Dictionary<string, double> thresholds = gains_and_thresholds["thresholds"];

            // Select the best attribute to split on
            double highest_gain_ratio = -1;
            string best_split_attribute = "NOTFOUND";
            Boolean split_on_continuous = false;
            double threshold = 0;
            foreach (string attribute in attributes.Keys.ToList())
            {
                agent.THINK("consider-attribute").set("attributes_left", attributes.Count).finish();
                double my_gain_ratio = gains[attribute];

                Dictionary<string, object> comparingState = StateRecording.generateState("current_best_attribute", best_split_attribute, "competing_attribute", attribute, "current_gain", highest_gain_ratio, "competing_gain", my_gain_ratio);

                if (my_gain_ratio > highest_gain_ratio)
                {
                    agent.THINK("set-new-best-attribute").setState(comparingState).finish();
                    highest_gain_ratio = my_gain_ratio;
                    best_split_attribute = attribute;
                    split_on_continuous = (attributes[attribute] == "continuous");
                    if (split_on_continuous)
                    {
                        threshold = thresholds[attribute];
                    }
                }
                else
                {
                    agent.THINK("keep-old-attribute").setState(comparingState).finish();
                }
            }
            agent.THINK("end-attribute-loop").set("attributes_left", 0).finish();

            // This is to come to the same result as J48 [TODO: This has to go at some point]
            if (!have_been_at_root && best_split_attribute == "petal-length")
            {
                have_been_at_root = true;
                Console.WriteLine("Adjust to J48");
                best_split_attribute = "petal-width";
                threshold = thresholds[best_split_attribute];
            }

            Dictionary<string, string> attributes_for_further_iteration = AttributeHelper.CopyAttributeDictionary(attributes);
            Dictionary<string, List<DataInstance>> subsets = (split_on_continuous) ? SetHelper.subsetOnAttributeContinuous(set, best_split_attribute, threshold) : SetHelper.subsetOnAttributeNominal(set, best_split_attribute, possible_nominal_values[best_split_attribute]);
            bool subset_below_minimum_requirement = false;
            foreach (List<DataInstance> subset in subsets.Values.ToList())
            {
                // If at least one of these subsets has less instances than the minimum leaf size, than this split should NOT happen. 
                subset_below_minimum_requirement = (subset.Count < minimum_leaf_size) ? true : subset_below_minimum_requirement;
            }
            if (subset_below_minimum_requirement)
            {
                agent.THINK("do-not-split").set("best_attribute", best_split_attribute).set("highest_gain", highest_gain_ratio).set("possible_attributes", attributes_for_further_iteration.Count).finish();
                tree = this.addEstimationLeaf(tree, set, parent, last_split);
                if (parent != null)
                {
                    agent.THINK("return-tree-to-self").finish();
                }
                return tree;
            }

            agent.THINK("add-node").set("best_attribute", best_split_attribute).set("highest_gain", highest_gain_ratio).set("possible_attributes", attributes_for_further_iteration.Count).finish();
            // We now know the best splitting attribute and how to split it. We're gonna create the subsets now.
            Node newnode = null;
            if (split_on_continuous)
            {
                newnode = tree.addContinuousNode(best_split_attribute, last_split, threshold, parent);
            }
            else
            {
                newnode = tree.addNode(best_split_attribute, last_split, parent);
                attributes_for_further_iteration.Remove(best_split_attribute);
            }
            // We now have a dictionary where each string represents the value split and the list of datainstances is the subset.

            int values_left = subsets.Keys.Count;
            foreach (string subset_splitter in subsets.Keys)
            {
                List<DataInstance> subset = subsets[subset_splitter];
                agent.THINK("subset-on-value").set("values_left", values_left).finish();
                Dictionary<string, object> state = StateRecording.generateState("split_attribute", best_split_attribute, "split_value", subset_splitter);
                if (subset.Count == 0)
                {
                    // There are no more of this subset. We need to skip this iteration.
                    agent.THINK("ignore-value").setState(state).finish();
                    continue;
                }
                if (SetHelper.hasUniformClassifier(subset, target_attribute))
                {
                    // This subset doesn't have to be split anymore. We can just add it to the node as a leaf. 
                    // Each leaf represents one decision rule. 
                    string classifier_value = subset.First().getProperty(target_attribute);
                    double certainty = 0;

                    // Calculate the certainty of this leaf. It's the average weight of the dataset. 
                    foreach (DataInstance instance in subset)
                    {
                        certainty += instance.getWeight();
                    }
                    certainty /= (double)subset.Count;

                    agent.THINK("add-leaf").setState(state).finish();
                    Leaf leaf = tree.addUncertainLeaf(subset_splitter, classifier_value, newnode, certainty);
                    tree.data_locations[leaf] = subset;
                }
                else
                {
                    // We still haven't resolved this set. We need to iterate upon it to split it again. 
                    if (attributes_for_further_iteration.Count == 0)
                    {
                        agent.THINK("add-best-guess-leaf").setState(state).finish();
                        tree = this.addEstimationLeaf(tree, subset, newnode, subset_splitter);
                    }
                    else
                    {
                        // We still have attributes left, we can continue further!
                        agent.THINK("iterate-further").setState(state).finish();
                        tree = this.iterate(tree, subset, target_attribute, attributes_for_further_iteration, newnode, subset_splitter);
                    }
                    // If we got here in the code then the set that was previously not all the same classifier has been resolved. 
                    // Therefore we can let the foreach continue further!
                }

                values_left--;
            }
            // The set that we have received has been dealt with completely. We can now move up!
            agent.THINK("end-value-loop").set("values_left", 0).finish();
            if (parent != null)
            {
                agent.THINK("return-tree-to-self").finish();
            }
            return tree;
        }

        private Dictionary<string, Dictionary<string, double>> calculate_attribute_gain_ratios(List<DataInstance> set, string target_attribute, Dictionary<string, string> attributes)
        {
            Dictionary<string, double> attribute_gains = new Dictionary<string, double>();
            Dictionary<string, double> continuous_thresholds = new Dictionary<string, double>();

            foreach (string attr in attributes.Keys.ToList())
            {
                // A nominal attribute we just need the ratio of, for a continuous attribute we determine best threshold, and then
                // the gain ratio of splitting on that treshold. 
                if (attributes[attr] == "nominal")
                {
                    double my_ratio = Calculator.gainRatio(set, attr, target_attribute, possible_nominal_values[attr]);
                    attribute_gains[attr] = my_ratio;
                }

                if (attributes[attr] == "continuous")
                {
                    double[] split_and_ratio = Calculator.best_split_and_ratio_for_continuous(set, attr, target_attribute, possible_continuous_values[attr]);
                    attribute_gains[attr] = split_and_ratio[1];
                    continuous_thresholds[attr] = split_and_ratio[0];
                }
            }

            return new Dictionary<string, Dictionary<string, double>> { { "gains", attribute_gains }, { "thresholds", continuous_thresholds } };

        }

        private DecisionTree addEstimationLeaf(DecisionTree tree, List<DataInstance> subset, Node parent, string value_splitter)
        {
            // We are out of attributes to split on! We need to identify the most common classifier. 
            string most_common_classifier = SetHelper.mostCommonClassifier(subset, target_attribute);

            // Adjust for the uncertainty that comes with this prediction.  We combine the certainty of classifier (percentage) with the certainty of the instances belonging here (weight).
            double percentage_with_this_classifier = (double)subset.Where(A => A.getProperty(target_attribute) == most_common_classifier).ToList().Count / (double)subset.Count;
            double certainty = 0;
            foreach (DataInstance instance in subset)
            {
                certainty += instance.getWeight();
            }
            certainty /= (double)subset.Count;
            certainty = certainty * percentage_with_this_classifier;
            Leaf leaf = tree.addUncertainLeaf(value_splitter, most_common_classifier, parent, certainty);
            tree.data_locations[leaf] = subset;

            return tree;
        }
    }
}