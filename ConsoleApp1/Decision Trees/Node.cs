using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    public class Node
    {

        public string label;
        private Node parent = null;
        private string value_splitter = "";
        private List<Node> nodeChildren = new List<Node>();
        private List<Leaf> leafChildren = new List<Leaf>();

        public Node(string label, string value_splitter)
        {
            this.label = label;
            this.value_splitter = value_splitter;
        }
        
        public void addChildNode(Node child)
        {
            this.nodeChildren.Add(child);
        }

        public void addParentNode(Node parent)
        {
            this.parent = parent;
        }

        public void addChildLeaf(Leaf leaf)
        {
            this.leafChildren.Add(leaf);
        }
        
        public string triggerRule(string value_splitter)
        {
            string add = " ";
            
            if (parent != null)
            {
                add = parent.triggerRule(this.value_splitter) + " AND ";
            }
            else
            {
                add = "IF ";
            }
            

            add += this.label + " = " + value_splitter;
            return add;
        }

        public Node getParent()
        {
            return this.parent;
        }
    }
}
