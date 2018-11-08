using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionAddBestGuessLeaf: Decision
    {
        public override Decision setAppliedAction(Dictionary<string, string> variables)
        {
            this.appliedaction = $"CREATE BEST GUESS LEAF FOR {variables["attribute_name"]} = {variables["attribute_value"]}";
            return this;
        }

        public override Decision setProof(Dictionary<string, string> variables)
        {
            this.proof = $"classifier({variables["attribute_name"]} = {variables["attribute_value"]}) = DISTINCT & AttributeCount = 0";
            return this;
        }

        protected override void setUtility()
        {
            this.utility_action = "Add New Best Guess Leaf";
            this.utility_premise = "Subset could not be further distinguished.";
        }
    }
}
