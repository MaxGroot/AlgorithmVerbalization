using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class SystemState
    {
        public List<object> variable_values = new List<object>();
        public List<string> variable_names = new List<string>();

        private EventDescriptor descriptor;

        public SystemState(params object[] parameters)
        {
            foreach(object variable in parameters)
            {
                variable_values.Add(variable);
            }

        }

        public SystemState setDescriptor(EventDescriptor descriptor)
        {
            this.descriptor = descriptor;
            this.variable_names = descriptor.variable_names;
            return this;
        }

        public EventDescriptor getDescriptor()
        {
            return this.descriptor;
        }

        public void setVariable(string name, object value, bool force_addition = false)
        {
            // At what position do we have the variable in our descriptor?
            int valueIndex = this.variable_names.IndexOf(name);
            // On that same position we update our value.
            if (valueIndex == -1)
            {
                if (!force_addition)
                {
                    throw new Exception($"Can't set {name} to {value} because it's not in my variable names contract.");
                }
                this.variable_values.Add(value);
                this.variable_names.Add(name);
            }else
            {
                Console.WriteLine($"{valueIndex}");
                foreach(string vname in variable_names)
                {
                    Console.WriteLine(name);
                }
                Console.WriteLine("POEP");
                foreach(object val in variable_values)
                {
                    Console.WriteLine(val.ToString());
                }
                this.variable_values[valueIndex] = value;
            }
        }

        public object getVariable(string name)
        {
            int valueIndex = this.descriptor.variable_names.IndexOf(name);
            if (valueIndex == -1)
            {
                return null;
            }
            return this.variable_values[valueIndex];
        }

        // Adds a new state to an old state. The old states variables are leading. 
        // When the new state does not contain the same variable name, the old one is used.
        // When the new state does contain the same variable, the value of the new one is used.
        public static SystemState Add(SystemState oldstate, SystemState newstate)
        {
            // Loop through old state's variable names
            foreach(string variable_name in oldstate.variable_names)
            {
                if (newstate.variable_names.Contains(variable_name))
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
            foreach(object obj in oldstate.variable_values)
            {
                object_list.Add(obj);
            }
            SystemState copy = new SystemState(object_list.ToArray());
            copy.setDescriptor(oldstate.getDescriptor());
            copy.variable_names = oldstate.variable_names;

            return copy;
        }
    }
}
