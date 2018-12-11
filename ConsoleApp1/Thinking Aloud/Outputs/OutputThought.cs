using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class OutputThought: Output
    {
        public string type;
        public string name;
        public SystemState state;

        public OutputThought(string type, string name, SystemState state)
        {
            this.type = type;
            this.name = name;
            this.state = state;
        }

        public string toLine(String seperator, SystemStateDescriptor total)
        {
            string addline = $"{this.type}{seperator}{this.name}{seperator} {seperator} {seperator}";
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
