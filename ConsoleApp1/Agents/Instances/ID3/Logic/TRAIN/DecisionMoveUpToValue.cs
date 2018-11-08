using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionMoveUpToValue: Decision
    {
        public override Decision setAppliedAction(Dictionary<string, string> variables)
        {
            this.appliedaction = $"MOVE UP TO VALUE. Back to {variables["parent_value"]}";
            return this;
        }

        public override Decision setProof(Dictionary<string, string> variables)
        {
            this.proof = $"{variables["attribute_name"]}  considered fully.";
            return this;
        }

        protected override void setUtility()
        {
            this.utility_action = "Move Up To Parent Value";
            this.utility_premise = "All subsets corresponding to values of this attribute have been resolved.";
        }
    }
}
