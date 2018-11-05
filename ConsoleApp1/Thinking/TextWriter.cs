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
        private List<string> infer_lines = new List<string>();
        private List<string> decision_lines = new List<string>();
        private List<string> model_lines = new List<string>();

        private List<Thought> thoughts = new List<Thought>();

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
        public void infer_add(SystemState state, string reason)
        {
            // Calculate new system state. 
            this.total_state = SystemState.Add(total_state, state);
            this.add_thought(ref infer_lines, "INFER", reason);
        }

        public void decision_add(string line)
        {
            this.add_thought(ref decision_lines, "DECIDE", line);
        }

        public void model_add(string line)
        {
            this.add_thought(ref model_lines, "PERFORM", line);
        }

        private void add_thought(ref List<string> list, string type, string line)
        {
            list.Add(line);

            // Remove tabs and double spaces from thoughts. 
            char tab = '\u0009';
            line = line.Replace(tab.ToString(), "");
            while (line.IndexOf("  ") > 0)
            {
                line = line.Replace("  ", " ");
            }


            // Since we do not want to refer to the same object (ruining the list), we copy the state we had before.
            SystemState my_state = SystemState.copy(total_state);
            this.thoughts.Add(new Thought(type, line, my_state));
            
        }
        public void write()
        {
            System.IO.File.WriteAllLines(location + "infers.txt", infer_lines.ToArray());
            System.IO.File.WriteAllLines(location + "model.txt", model_lines.ToArray());
            System.IO.File.WriteAllLines(location + "decisions.txt", decision_lines.ToArray());
            this.save_thoughts();
        }

        public void save_thoughts()
        {
            string seperator = ",";
            var csv = new StringBuilder();
            string firstline = $"Type{seperator}Value";
            foreach(string variable_name in this.total_descriptor.variable_names)
            {
                firstline += $"{seperator}{variable_name}";
            }
            firstline += seperator;
            csv.AppendLine(firstline);

            foreach(Thought thought in thoughts)
            {
                string addline = $"{thought.type}{seperator}{thought.value}";
                foreach (string variable_name in this.total_descriptor.variable_names)
                {
                    string value = thought.state_of_thought.getVariable(variable_name).ToString();
                    addline += $"{seperator}{value}";
                }
                addline += seperator;
                csv.AppendLine(addline);
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
