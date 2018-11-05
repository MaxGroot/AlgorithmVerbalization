using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class SystemState
    {
        public List<object> state = new List<object>();
        private SystemStateDescriptor descriptor;

        public SystemState(params object[] parameters)
        {
            foreach(object variable in parameters)
            {
                state.Add(variable);
            }

        }

        public SystemState setDescriptor(SystemStateDescriptor descriptor)
        {
            this.descriptor = descriptor;
            return this;
        }

        public SystemStateDescriptor getDescriptor()
        {
            return this.descriptor;
        }

        public void setVariable(string name, object value)
        {
            // At what position do we have the variable in our descriptor?
            int valueIndex = this.descriptor.variable_names.IndexOf(name);
            // On that same position we update our value.
            this.state[valueIndex] = value;

        }

        public object getVariable(string name)
        {
            int valueIndex = this.descriptor.variable_names.IndexOf(name);
            return this.state[valueIndex];
        }

        // Adds a new state to an old state. The old states variables are leading. 
        // When the new state does not contain the same variable name, the old one is used.
        // When the new state does contain the same variable, the value of the new one is used.
        public static SystemState Add(SystemState oldstate, SystemState newstate)
        {
            // Loop through old state's variable names
            foreach(string variable_name in oldstate.descriptor.variable_names)
            {
                if (newstate.descriptor.variable_names.Contains(variable_name))
                {
                    // The new state knows about the old state's variable. Hence, we update the old one's value.
                    oldstate.setVariable(variable_name, newstate.getVariable(variable_name));
                }
            }
            return oldstate;
        }

        public static SystemState copy(SystemState oldstate)
        {
            List<object> object_list = new List<object>();
            foreach(object obj in oldstate.state)
            {
                object_list.Add(obj);
            }
            SystemState copy = new SystemState(object_list.ToArray());
            copy.setDescriptor(oldstate.getDescriptor());

            return copy;
        }
    }
}
