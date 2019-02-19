using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class SystemState
    {
        private List<object> variable_values;
        private List<string> variable_names;
        private string identifier;
        private EventDescriptor descriptor;

        public SystemState(params object[] parameters)
        {

            this.variable_values = new List<object>();
            this.variable_names = new List<string>();

            foreach (object variable in parameters)
            {
                variable_values.Add(variable);
            }
            this.identifier = "UNASSIGNED";
        }
        private void add_pair(string name, object value)
        {
            this.variable_names.Add(name);
            this.variable_values.Add(value);
        }
        private void clear_all_pairs()
        {
            this.variable_names.Clear();
            this.variable_values.Clear();
        }
        private void set_empty_pairs(List<string> names)
        {
            foreach(string name in names)
            {
                this.add_pair(name, "");
            }
        }
        public SystemState setDescriptor(EventDescriptor descriptor)
        {
            this.descriptor = descriptor;
            this.set_empty_pairs(descriptor.variable_names);
            return this;
        }
        public SystemState setIdentifier(string identifier)
        {
            this.identifier = identifier;
            return this;
        }
        public string getIdentifier()
        {
            return this.identifier;
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
                this.add_pair(name, value);
                
            }
            else
            {
                if (this.variable_values.Count - 1 >= valueIndex)
                {
                    this.variable_values[valueIndex] = value;
                }else
                {
                    throw new Exception("Mismatch between variable values and variable names!");
                }
            }
        }

        public object getVariable(string name)
        {
            int valueIndex = this.variable_names.IndexOf(name);
            if (valueIndex == -1)
            {
                return null;
            }
            return this.variable_values[valueIndex];
        }

        public void write()
        {
            string outt = "";
            foreach (string n in this.variable_names)
            {
                outt += " - " + n;
            }
            Console.WriteLine($"Names: {outt}");
            outt = "";
            foreach (object n in variable_values)
            {
                outt += " - " + n.ToString();
            }
            Console.WriteLine($"Variables: {outt}");
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

        public static SystemState copy(SystemState oldstate, string identifier)
        {
            List<object> object_list = new List<object>();
            foreach(object obj in oldstate.variable_values)
            {
                object_list.Add(obj);
            }
            SystemState copy = new SystemState(object_list.ToArray());
            copy.setDescriptor(oldstate.getDescriptor());
            copy.clear_all_pairs();
            
            foreach(string name in oldstate.variable_names)
            {
                copy.add_pair(name, oldstate.variable_values[oldstate.variable_names.IndexOf(name)]);
            }

            copy.setIdentifier(identifier);
            return copy;
        }
    }
}
