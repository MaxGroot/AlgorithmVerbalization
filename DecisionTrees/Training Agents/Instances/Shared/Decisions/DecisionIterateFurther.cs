using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionIterateFurther: Decision
    {
        public override Decision setAppliedAction(Dictionary<string, string> variables)
        {
            this.appliedaction = $"ITERATE FURTHER ON {variables["attribute_name"]} = {variables["attribute_value"]}";
            return this;
        }

        public override Decision setProof(Dictionary<string, string> variables)
        {
            this.proof = $"classifier({variables["attribute_name"]} = {variables["attribute_value"]}) = DISTINCT";
            return this;
        }

        protected override void setUtility()
        {
            this.utility_action = "Iterate further";
            this.utility_premise = "Subset not fully classified on this value.";
        }
    }
}
