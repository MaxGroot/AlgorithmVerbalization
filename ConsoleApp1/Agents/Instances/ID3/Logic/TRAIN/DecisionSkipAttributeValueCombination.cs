using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionSkipAttributeValueCombination: Decision
    {
        public override Decision setAppliedAction(Dictionary<string, string> variables)
        {
            this.appliedaction = $"NOTHING TO DO FOR {variables["attribute_name"]} = {variables["attribute_value"]}";
            return this;
        }

        public override Decision setProof(Dictionary<string, string> variables)
        {
            this.proof = $"count({variables["attribute_name"]} = {variables["attribute_value"]}) = 0";
            return this;
        }

        protected override void setUtility()
        {
            this.utility_action = "Skip Attribute-Value Combination";
            this.utility_premise = "No attribute-value combination of this kind in this subset.";
        }
    }
}
