﻿using System;
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
                throw new Exception($"Cannot set {name}, descriptor {descriptor.id} has no such key");
            }
            if (! verifyType(descriptor.considerations[name], value))
            {
                throw new Exception($"Cannot set {value} for {name}, that variable has {descriptor.considerations[name]} as a type");
            }
            this.state[name] = value;
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
            foreach(string variableName in descriptor.considerations.Keys.ToList())
            {
                if (! state.ContainsKey(variableName))
                {
                    throw new Exception($"Cannot finish recording: {variableName} is missing.");
                }
            }
            this.finished = true;
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
    }
}
