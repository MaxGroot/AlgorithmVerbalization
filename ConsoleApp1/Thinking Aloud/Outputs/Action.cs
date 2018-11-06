using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    // TODO: Rename Action to Decision
    class Action: Output
    {
        public string type = "DECIDE";
        public SystemState state;
        public Decision explanation;

        public Action(Decision explanation, SystemState state)
        {
            this.explanation = explanation;
            this.state = state;
        }

        public string toLine(string seperator, SystemStateDescriptor total)
        {

            string addline = $"{this.type}{seperator}{explanation.utility_action}{seperator}{explanation.utility_premise}{seperator}{explanation.proof}{seperator}{explanation.appliedaction}";
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
