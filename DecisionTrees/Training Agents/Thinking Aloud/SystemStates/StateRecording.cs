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
        public string action;
        private Dictionary<string, object> state = new Dictionary<string, object>();
        private bool finished = false;

        public StateRecording(string action, StateDescriptor descriptor)
        {
            this.action = action;
            this.descriptor = descriptor;
        }

        public StateRecording set(string name, object value)
        {
            if (! descriptor.considerations.ContainsKey(name))
            {
                throw new Exception($"Cannot set {name} for {action}, as descriptor {descriptor.id} has no such key");
            }
            if (! verifyType(descriptor.considerations[name], value))
            {
                throw new Exception($"Cannot set {value} for variable {name} of {action}, that variable has {descriptor.considerations[name]} as a type");
            }
            this.state[name] = value;
            return this;
        }
        public StateRecording setState(Dictionary<string, object> state)
        {
            foreach(string key in state.Keys.ToList())
            {
                object value = state[key]; 
                if (!descriptor.considerations.ContainsKey(key))
                {
                    throw new Exception($"Cannot set {key} for {action}, as descriptor {descriptor.id} has no such key");
                }
                if (!verifyType(descriptor.considerations[key], value))
                {
                    throw new Exception($"Cannot set {value} for variable {key} of {action}, that variable has {descriptor.considerations[key]} as a type");
                }
            }
            this.state = state;
            return this;
        }
        public object getVariable(string name)
        {
            if (! state.ContainsKey(name))
            {
                throw new Exception($"{name} was not set for this state recording");
            }

            return this.state[name];
        }

        public void finish()
        {
            if (this.descriptor == null)
            {
                this.finished = true;
                return;
            }
            foreach(string variableName in descriptor.considerations.Keys.ToList())
            {
                if (! state.ContainsKey(variableName))
                {
                    throw new Exception($"Cannot finish recording: {variableName} is missing.");
                }
            }
            this.finished = true;
        }

        public string toLine(string seperator)
        {
            if (!this.finished)
            {
                throw new Exception($"The program attempts to save an improperly configured StateRecording with action {action}");
            }
            string ret = "";
            foreach(string variableName in this.state.Keys.ToList())
            {
                ret += $"{this.state[variableName]}{seperator}";
            }
            ret += $"{this.action}{seperator}";
            return ret;
        }

        protected static bool verifyType(string type, object check)
        {
            switch(type)
            {
                case "string":  return check is string;
                case "double":  return check is double;
                case "float":   return check is float;
                case "int":     return check is int;
                default: throw new Exception($"{type} not supported as a variable type for state variables.");
            }
        }

        public static Dictionary<string, object> generateState(params object[] statevariables)
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            string previous_key = "UNSET";
            for(int i =0; i<statevariables.Length; i++)
            {
                if (i % 2 == 0)
                {
                    // This is a key
                   previous_key = (string)statevariables[i]; 
                }
                else
                {
                    // This is a value
                    if (previous_key == "UNSET")
                    {
                        throw new Exception("Value supplied without supplying a key first.");
                    }
                    state[previous_key] = statevariables[i];
                    previous_key = "UNSET";
                }
            }

            return state;
        }
    }
}
