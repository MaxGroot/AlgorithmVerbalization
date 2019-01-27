using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class C45Algorithm : Algorithm
    {
        public DecisionTree train(List<DataInstance> examples, string target_attribute, List<string> attributes, Agent runner)
        {
            throw new NotImplementedException();
        }

        private DecisionTree iterate(DecisionTree tree, List<DataInstance> set, string target_attribute, List<string> attributes, Agent runner)
        {
            // Check termination criteria
            if (termination_criteria_met(set, target_attribute))
            {
                return tree;
            }
            foreach(string attr in attributes)
            {
                double gainratio = Calculator.splitInfo()
            }
            // Compute criteria for attributes
            double global_entropy = Calculator.entropy(set, target_attribute);

            // Choose best attribute

            // Create node for attribute

            // Split the set based on newly created node

            // For all sub data, call C4.5 to get a sub-tree

            // Attach sub-tree to the node of 4.

            // Return tree
            return tree;
        }


        private bool termination_criteria_met(List<DataInstance> set, string target_attribute)
        {
            if (Calculator.subset_has_all_same_classifier(set, target_attribute))
            {
                return true;
            }

            return false;
        }
    }
}
