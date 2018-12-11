using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    abstract class Decision
    {
        public string utility_premise;
        public string utility_action;
        public string proof = "";
        public string appliedaction = "";
        public Decision()
        {
            this.setUtility();
        }
        protected abstract void setUtility();

        public abstract Decision setProof(Dictionary<string, string> variables);

        public abstract Decision setAppliedAction(Dictionary<string, string> variables);
    }
}
