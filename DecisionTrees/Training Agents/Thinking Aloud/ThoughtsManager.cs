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
        private int state_counter;

        private Dictionary<EventDescriptor, int> last_output_position_with_this_descriptor = new Dictionary<EventDescriptor, int>();

        public ThoughtsManager()
        {
            state_counter = 0;
        }

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
            
            Console.WriteLine("Initial System State and Posisble System State Attributes Recorded");
        }

        public void add_thought(string occurence, string action, SystemState state)
        {
            // Since we do not want to refer to the same object (ruining the list), we copy the state we had before.
            state_counter++;
            SystemState my_state = SystemState.copy(state, Calculator.generateElementId(this.state_counter));

            this.outputs.Add(new Output(occurence, action, my_state));
            this.last_output_position_with_this_descriptor[state.getDescriptor()] = this.outputs.Count - 1;
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
                // Insert variable dependencies
                Output new_output = this.insert_dependencies(output, last_output_position_with_this_descriptor, outputs);
                
                // Append
                csv.AppendLine(new_output.toLine(seperator, this.total_descriptor));
            }

            return csv.ToString();
        }

        private Output insert_dependencies(Output output_to_update, Dictionary<EventDescriptor, int> last_output_position_with_this_descriptor, List<Output> all_outputs)
        {
            foreach(EventDescriptor desc in output_to_update.state.getDescriptor().dependencies)
            {
                Console.WriteLine($"{desc.cause} -  dependency for {output_to_update.state.getDescriptor().cause}: ");
                Output last_output = all_outputs[last_output_position_with_this_descriptor[desc]];
                Console.WriteLine($"Output found in {last_output.state.getDescriptor().cause}..");
                foreach(string key in desc.variable_names)
                {
                    Console.WriteLine($"Getting and setting variable {key}..");
                    string old_value = last_output.state.getVariable(key).ToString();
                    Console.WriteLine($"Old value: {old_value}");
                    output_to_update.state.setVariable(key, old_value, true);
                }
            }
            return output_to_update;
        }
    }
}
