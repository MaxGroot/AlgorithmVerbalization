using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Node: DecisionTreeElement
    {
        private string label;
        private Dictionary<string, DecisionTreeElement> children;

        public Node(string label)
        {
            this.label = label;
        }

        public string getLabel()
        {
            return this.label;
        }
        public void handleInstance(DataInstance instance)
        {
            throw new NotImplementedException();
        }
    }
}
