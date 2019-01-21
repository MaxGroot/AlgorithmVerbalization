using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class Output
    {
        public string occurence;
        public string action;
        public SystemState state;

        public Output(string occurence, string action, SystemState state)
        {
            this.occurence = occurence;
            this.action = action;
            this.state = state;
        }

        public string toLine(String seperator, SystemStateDescriptor total)
        {
            string addline = $"{this.occurence}{seperator}{this.action}";
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
