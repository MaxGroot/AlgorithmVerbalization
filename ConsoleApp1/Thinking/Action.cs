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
        public string utility_action;
        public string type = "DECIDE";
        public string utility_premise;
        public string proof;
        public string applied_action;
        public SystemState state;

        public Action(string utility_action, string utility_premise, string proof, string applied_action, SystemState state)
        {
            this.utility_action = utility_action;
            this.utility_premise = utility_premise;
            this.proof = proof;
            this.applied_action = applied_action;
            this.state = state;
        }

        public string toLine(string seperator, SystemStateDescriptor total)
        {

            string addline = $"{this.type}{seperator}{utility_action}{seperator}{utility_premise}{seperator}{proof}{seperator}{applied_action}";
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
