using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ThoughtsManager
    {
        private List<Output> outputs = new List<Output>();

        private List<EventDescriptor> all_descriptors = new List<EventDescriptor>();
        private EventDescriptor total_descriptor;
        private SystemState total_state;

        public void add_systemstate_descriptor(EventDescriptor descriptor)
        {
            all_descriptors.Add(descriptor);
        }

        public void generate_total_descriptor()
        {
            this.total_descriptor = EventDescriptor.generateTotal(all_descriptors);
            List<object> current_objects = new List<object>();
            foreach (string name in this.total_descriptor.variable_names)
            {
                current_objects.Add("");
            }

            this.total_state = new SystemState(current_objects.ToArray());
            total_state.setDescriptor(total_descriptor);
            Console.WriteLine("Initial System State and Posisble System State Attributes Recorded");
        }

        public void add_thought(string occurence, string action, SystemState state)
        {
            // Since we do not want to refer to the same object (ruining the list), we copy the state we had before.
            SystemState my_state = SystemState.copy(state);
            this.total_state = SystemState.Add(total_state, state);
            this.outputs.Add(new Output(occurence, action, my_state));
        }
        public string output()
        {
            string seperator = ";";
            var csv = new StringBuilder();
            string firstline = $"Event{seperator}Action";
            foreach (string variable_name in this.total_descriptor.variable_names)
            {
                firstline += $"{seperator}{variable_name}";
            }
            firstline += seperator;
            csv.AppendLine(firstline);

            foreach (Output output in outputs)
            {
                csv.AppendLine(output.toLine(seperator, this.total_descriptor));
            }

            return csv.ToString();
        }
    }
}
