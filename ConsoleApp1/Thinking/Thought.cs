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

        public Thought(string type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
}
