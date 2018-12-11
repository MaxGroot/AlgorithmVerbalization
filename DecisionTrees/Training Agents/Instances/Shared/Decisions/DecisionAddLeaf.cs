using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionAddLeaf: Decision
    {

        public override Decision setAppliedAction(Dictionary<string, string> variables)
        {
            this.appliedaction = $"CREATE LEAF FOR {variables["attribute_name"]} = {variables["attribute_value"]}";
            return this;
        }

        public override Decision setProof(Dictionary<string, string> variables)
        {
            this.proof = $"classifier({variables["attribute_name"]} = {variables["attribute_value"]}) = {variables["classifier_value"]}";
            return this;
        }

        protected override void setUtility()
        {
            this.utility_action = "Add New Leaf";
            this.utility_premise = "Subset perfectly classified.";
        }
    }
}
