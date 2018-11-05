using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class Thought: Output
    {
        public string type;
        public string name;
        public SystemState state_of_thought;

        public Thought(string type, string name, SystemState state_of_thought)
        {
            this.type = type;
            this.name = name;
            // TODO: Rename to state
            this.state_of_thought = state_of_thought;
        }

        public string toLine(String seperator, SystemStateDescriptor total)
        {
            string addline = $"{this.type}{seperator}{this.name}{seperator} {seperator} {seperator}";
            foreach (string variable_name in total.variable_names)
            {
                string value = this.state_of_thought.getVariable(variable_name).ToString();
                addline += $"{seperator}{value}";
            }
            addline += seperator;
            return addline;
        }
    }
}
