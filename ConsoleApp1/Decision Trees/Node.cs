using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Node
    {

        public string label;
        private Node parent;
        private List<Node> children = new List<Node> ();

        public Node(string label)
        {
            this.label = label;
        }
        
        public void addChildNode(Node child)
        {
            this.children.Add(child);
        }

        public void addParentNode(Node parent)
        {
            this.parent = parent;
        }
        
    }
}
