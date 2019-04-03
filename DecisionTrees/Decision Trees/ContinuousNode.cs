using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ContinuousNode : Node
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
    }
}
