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
        private string location;

        public TextWriter(string location)
        {
            this.location = location;
        }

        public void filesave_string(string filename, string output)
        {
            File.WriteAllText(location + filename, output);
        }

        public void filesave_lines(string filename, List<string> lines)
        {
            File.WriteAllLines(location + filename, lines.ToArray());
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
