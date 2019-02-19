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
        private string save_location = "";
        private IniFile ini;

        public TextWriter()
        {
            string directory = System.AppDomain.CurrentDomain.BaseDirectory + "../../../";
            string my_path = directory + "env.ini";
            if (!System.IO.File.Exists(my_path))
            {
                my_path = directory + "config.ini";
                if (!System.IO.File.Exists(my_path))
                {
                    Console.WriteLine(my_path);
                    throw new Exception("Neither an environment configuration or a default configuration file found.");
                }
                Console.WriteLine("Env file could not be found, switching to default configuration instead.");
            }
            this.ini = new IniFile(my_path);
        }
        
        public void filesave_string(string filename, string output)
        {
            bool keep_trying = true;
            while(keep_trying)
            {
                try
                {
                    File.WriteAllText(save_location + filename, output);
                    keep_trying = false;
                }
                catch(IOException e)
                {
                    Console.WriteLine($"Could not save file {filename}, in use. Try again? Press enter for yes, anything else for no.");
                    string input = Console.ReadLine();
                    if (input != "")
                    {
                        keep_trying = false;
                        Console.WriteLine($"Not saving {filename}.");
                    }
                }
            }

        }

        public void filesave_lines(string filename, List<string> lines)
        {
            File.WriteAllLines(save_location + filename, lines.ToArray());
        }

        public void set_location(string location)
        {
            this.save_location = location;
        }

        public string askFromConfig(string question, string section, string key)
        {
            string config = this.ini.Read(key, section);
            
            if (config == "" || section == "ASK ON STARTUP")
            {
                question = (config == "") ? question : question + " Press ENTER for [" + config + "]";
                Console.WriteLine(question);
                string input = Console.ReadLine();
                return (input == "") ? config : input;
            }
            return config;
        }
    }
}
