using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class StateRecording
    {
        private StateDescriptor descriptor;
        private Dictionary<string, object> state;

        public StateRecording(StateDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        public void setVariable(string name, object value)
        {
            if (! descriptor.considerations.ContainsKey(name))
            {
                throw new Exception($"Cannot set {name}, descriptor {descriptor.id} has no such key");
            }
            if (! verifyType(descriptor.considerations[name], value))
            {
                throw new Exception($"Cannot set {value} for {name}, that variable has {descriptor.considerations[name]} as a type");
            }
            this.state[name] = value;
        }

        public object getVariable(string name)
        {
            if (! state.ContainsKey(name))
            {
                throw new Exception($"{name} was not set for this state recording");
            }

            return this.state[name];
        }

        protected static bool verifyType(string type, object check)
        {
            switch(type)
            {
                case "string":  return check is string;
                case "double":  return check is double;
                case "float":   return check is float;
                default: return false;
            }
        }
    }
}
