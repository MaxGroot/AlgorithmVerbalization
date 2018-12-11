using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class OutputDecision: Output
    {
        public string type = "DECIDE";
        public SystemState state;
        public Decision decision;

        public OutputDecision(Decision decision, SystemState state)
        {
            this.decision = decision;
            this.state = state;
        }

        public string toLine(string seperator, SystemStateDescriptor total)
        {

            string addline = $"{this.type}{seperator}{decision.utility_action}{seperator}{decision.utility_premise}{seperator}{decision.proof}{seperator}{decision.appliedaction}";
            foreach (string variable_name in total.variable_names)
            {
                string value = this.state.getVariable(variable_name).ToString();
                addline += $"{seperator}{value}";
            }
            addline += seperator;
            return addline;
        }
    }
}
