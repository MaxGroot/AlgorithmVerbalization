using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class StateDescriptor
    {
        public string id = "UNSET";
        public Dictionary<string, string> considerations = new Dictionary<string, string>();
    }
}
