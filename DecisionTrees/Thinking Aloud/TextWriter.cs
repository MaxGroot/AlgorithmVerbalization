using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class TextWriter
    {
        private List<Output> outputs = new List<Output>();

        private List<SystemStateDescriptor> all_descriptors = new List<SystemStateDescriptor>();
        private SystemStateDescriptor total_descriptor;
        private SystemState total_state;

        private string location;

        public TextWriter(string location)
        {
            this.location = location;
        }
        public void add_systemstate_descriptor(SystemStateDescriptor descriptor)
        {
            all_descriptors.Add(descriptor);
        }
        public void generate_total_descriptor()
        {
            this.total_descriptor = SystemStateDescriptor.generateTotal(all_descriptors);
            List<object> current_objects = new List<object>();
            foreach(string name in this.total_descriptor.variable_names)
            {
                current_objects.Add("");
            }
            
            this.total_state = new SystemState(current_objects.ToArray());
            total_state.setDescriptor(total_descriptor);
            Console.WriteLine("Initial System State and Posisble System State Attributes Recorded");
        }
        public void infer_add(SystemState state)
        {
            // Calculate new system state. 
            this.total_state = SystemState.Add(total_state, state);
            this.add_thought(state.getDescriptor().name);
        }

        public void decision_add(Decision explanation)
        {
            this.add_action(explanation);
        }
        private void add_action(Decision explanation)
        {
            string applied_action = explanation.appliedaction;

            // Remove tabs and double spaces from the applied action. We only do this after adding it to the reference list, 
            // Because some lists want their applied_actions tabbed.
            char tab = '\u0009';
            applied_action = applied_action.Replace(tab.ToString(), "");
            while (applied_action.IndexOf("  ") > 0)
            {
                applied_action = applied_action.Replace("  ", " ");
            }

            // Put the tab-removed applied-action back into the explanation.

            explanation.appliedaction = applied_action;
            // Since we do not want to refer to the same object (ruining the list), we copy the state we had before.
            SystemState my_state = SystemState.copy(total_state);
            this.outputs.Add(new OutputDecision(explanation, my_state));
        }

        private void add_thought(string name)
        {        
            // Since we do not want to refer to the same object (ruining the list), we copy the state we had before.
            SystemState my_state = SystemState.copy(total_state);
            this.outputs.Add(new OutputThought("INFER", name, my_state));
            
        }
        public void write()
        {
            this.save_thoughts();

        }

        public void save_thoughts()
        {
            string seperator = ";";
            var csv = new StringBuilder();
            string firstline = $"Type{seperator}Utility Action{seperator}Utility Premise{seperator}Proof{seperator}Applied Action";
            foreach(string variable_name in this.total_descriptor.variable_names)
            {
                firstline += $"{seperator}{variable_name}";
            }
            firstline += seperator;
            csv.AppendLine(firstline);

            foreach(Output output in outputs)
            {
                csv.AppendLine(output.toLine(seperator, this.total_descriptor));
            }
            File.WriteAllText(location + "thoughts.csv", csv.ToString());
        }
        public static string askLocation(int output_line)
        {
            string ask = Console.ReadLine();
            if (ask == "")
            {
                string my_path = System.AppDomain.CurrentDomain.BaseDirectory + "../../../";

                if (!System.IO.File.Exists(my_path + ".env"))
                {
                    Console.WriteLine(my_path + ".env");
                    throw new Exception("No input supplied but no env file found either");
                }

                var sr = new StreamReader(my_path + ".env");

                for (int i = 1; i <= output_line; i++)
                {
                    string this_line = sr.ReadLine();

                    if (i == output_line)
                    {
                        return this_line;
                    }
                }

                throw new Exception("Env file did not contain specified line");
                    
            }
            return ask;
        }
    }
}
