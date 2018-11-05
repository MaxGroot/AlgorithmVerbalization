using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class Thought
    {
        public string type;
        public string value;
        public SystemState state_of_thought;

        public Thought(string type, string value, SystemState state_of_thought)
        {
            this.type = type;
            this.value = value;
            this.state_of_thought = state_of_thought;
        }

        public void state_gain()
        {
            Console.WriteLine(this.state_of_thought.getVariable("my_gain"));
        }
    }
}
