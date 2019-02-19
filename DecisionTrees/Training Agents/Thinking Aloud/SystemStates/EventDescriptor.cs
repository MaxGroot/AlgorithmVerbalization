using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class EventDescriptor
    {
        public List<string> variable_names = new List<string>();
        public List<EventDescriptor> dependencies = new List<EventDescriptor>();
        public string cause;
        public string name;
        public EventDescriptor(string cause, string name, List<string> variable_names, List<EventDescriptor> dependencies)
        {
            this.cause = cause;
            this.name = name;
            this.variable_names = variable_names;
            this.dependencies = dependencies;
        }

        public static EventDescriptor generateTotal(List<EventDescriptor> list)
        {
            List<string> variable_names = new List<string>();
            foreach(EventDescriptor descriptor in list)
            {
                foreach(string name in descriptor.variable_names)
                {
                    if (!variable_names.Contains(name))
                    {
                        variable_names.Add(name);
                    }
                }
            }

            return new EventDescriptor("This-is-irrelevant", "Total Descriptor", variable_names, new List<EventDescriptor>() { });
        }
    }
}
