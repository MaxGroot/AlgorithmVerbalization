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
                object getValue = this.state.getVariable(variable_name);
                string value_as_string = "";
                if (getValue != null)
                {
                    value_as_string = getValue.ToString();
                }
                addline += $"{seperator}{value_as_string}";
            }
            addline += seperator;
            return addline;
        }
    }
}
