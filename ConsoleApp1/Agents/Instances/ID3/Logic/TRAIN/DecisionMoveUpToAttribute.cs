using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionMoveupToAttribute: Decision
    {
        public override Decision setAppliedAction(Dictionary<string, string> variables)
        {
            this.appliedaction = $"MOVE UP TO ATTRIBUTE, Back to {variables["parent_attribute"]}";
            return this;
        }

        public override Decision setProof(Dictionary<string, string> variables)
        {
            this.proof = $"{variables["attribute_name"]} = {variables["attribute_value"]} considered fully.";
            return this;
        }

        protected override void setUtility()
        {
            this.utility_action = "Move Up To Parent Attribute";
            this.utility_premise = "The complete subset of this attribute-value combination has been resolved.";
        }
    }
}
