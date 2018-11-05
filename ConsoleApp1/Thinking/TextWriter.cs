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

        private string location;

        public TextWriter(string location)
        {
            this.location = location;

        }
        public void infer_add(string line)
        {
            this.add_thought(ref infer_lines, "INFER", line);
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

            this.thoughts.Add(new Thought(type, line));
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
            csv.AppendLine($"Type{seperator}Value{seperator}");
            foreach(Thought thought in thoughts)
            {
                csv.AppendLine($"{thought.type}{seperator}{thought.value}{seperator}");
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
