using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionAddNode : Decision
    {

        public override Decision setAppliedAction(Dictionary<string, string> variables)
        {
            this.appliedaction = $"CREATE NODE FOR {variables["attribute_name"]}";

            return this;
        }

        public override Decision setProof(Dictionary<string, string> variables)
        {
            this.proof = $"gain({variables["attribute_name"]}) = {variables["attribute_gain"]}";
            return this;
        }

        protected override void setUtility()
        {
            this.utility_action = "Add New Node";
            this.utility_premise = "Subset perfectly classified.";
        }
    }
}
