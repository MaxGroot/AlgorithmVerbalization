using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    public class ContinuousNode : Node
    {
        // A Continuous node has the following properties:
        // Identifier: Unique ID
        // Label: Which attribute is split on
        // Value_splitter: How does this node split from the previous node (which value)
        // Threshold: What is the threshold that this attribute is binarily split on.
        public double threshold;

        public ContinuousNode(string identifier, string label, string value_splitter) : base(identifier, label, value_splitter)
        {
        }

        public ContinuousNode setThreshold(double threshold)
        {
            this.threshold = threshold;
            return this;
        }

        public override string classify(DataInstance instance)
        {
            double instance_value = instance.getPropertyAsDouble(this.label);
            bool instance_is_below_or_equals = (instance_value <= this.threshold);
            bool instance_is_above = (instance_value > this.threshold);

            foreach(Node child in this.getNodeChildren())
            {
                bool child_filter_below_or_equals = child.value_splitter.Contains("<=");
                bool child_filter_above = child.value_splitter.Contains(">");

                if ((instance_is_above && child_filter_above) || (instance_is_below_or_equals && child_filter_below_or_equals))
                {
                    return child.classify(instance);
                }
            }

            foreach (Leaf child in this.getLeafChildren())
            {
                bool child_filter_below_or_equals = child.value_splitter.Contains("<=");
                bool child_filter_above = child.value_splitter.Contains(">");

                if ((instance_is_above && child_filter_above) || (instance_is_below_or_equals && child_filter_below_or_equals))
                {
                    return child.classifier;
                }
            }

            // If we got here, neither a leaf child nor a node child can accept this instance.
            throw new Exception($"Unknown value {instance_value} for {this.label} and threshold {this.threshold}");
        }
    }
}
