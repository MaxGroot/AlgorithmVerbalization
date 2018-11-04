using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class TextWriter
    {
        private List<string> lines = new List<string>();
        private string location;
        public TextWriter(string location)
        {
            this.location = location;
        }
        public void add(string line)
        {
            lines.Add(line);
        }

        public void write()
        {
            System.IO.File.WriteAllLines(location + "lines.txt", lines.ToArray());
        }
    }
}
