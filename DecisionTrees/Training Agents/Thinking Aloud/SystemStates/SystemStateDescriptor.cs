using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class SystemStateDescriptor
    {
        public List<string> variable_names = new List<string>();
        public string cause;
        public string name;
        public SystemStateDescriptor(string cause, string name, List<string> variable_names)
        {
            this.cause = cause;
            this.name = name;
            this.variable_names = variable_names;
        }

        public static SystemStateDescriptor generateTotal(List<SystemStateDescriptor> list)
        {
            List<string> variable_names = new List<string>();
            foreach(SystemStateDescriptor descriptor in list)
            {
                foreach(string name in descriptor.variable_names)
                {
                    if (!variable_names.Contains(name))
                    {
                        variable_names.Add(name);
                    }
                }
            }

            return new SystemStateDescriptor("This-is-irrelevant", "Total Descriptor", variable_names);
        }
    }
}
