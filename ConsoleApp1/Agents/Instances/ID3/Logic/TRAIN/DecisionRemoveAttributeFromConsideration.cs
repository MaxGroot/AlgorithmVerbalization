using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionRemoveAttributeFromConsideration : Decision
    {
        public override Decision setAppliedAction(Dictionary<string, string> variables)
        {
            this.appliedaction = $"REMOVE {variables["attribute_name"]} FROM SPLIT CONSIDERATION";

            return this;
        }

        public override Decision setProof(Dictionary<string, string> variables)
        {
            this.proof = $"gain({variables["attribute_name"]}) = {variables["attribute_gain"]}";
            return this;
        }

        protected override void setUtility()
        {
            this.utility_action = "Remove Attribute from consideration";
            this.utility_premise = "Highest gain has been determined.";
        }
    }
}
