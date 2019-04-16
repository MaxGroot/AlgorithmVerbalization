using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    public class Node: ITreeElement
    {

        public string label;
        private Node parent = null;
        public string value_splitter = "";
        public string identifier;
        private List<Node> nodeChildren = new List<Node>();
        private List<Leaf> leafChildren = new List<Leaf>();

        public Node(string identifier, string label, string value_splitter)
        {
            this.identifier = identifier;
            this.label = label;
            this.value_splitter = value_splitter;
        }
        
        public void addChildNode(Node child)
        {
            this.nodeChildren.Add(child);
        }

        public void removeChildNode(Node child)
        {
            for(int i =0; i<this.nodeChildren.Count; i++)
            {
                Node node = this.nodeChildren[i];

                if (node.identifier == child.identifier)
                {
                    nodeChildren.RemoveAt(i);
                    return;
                }
            }
            throw new Exception($"Child removal request on a node that doesn't have it: {child.identifier}");
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
        public List<Node> getNodeChildren()
        {
            return this.nodeChildren;
        }
        public List<Leaf> getLeafChildren()
        {
            return this.leafChildren;
        }
        public string line()
        {
            return this.value_splitter;
        }
        public string underline()
        {
            return this.label;
        }
        public List<ITreeElement> getAllChildren()
        {
            List<ITreeElement> ret = new List<ITreeElement>();
            ret.AddRange(this.getLeafChildren());
            ret.AddRange(this.getNodeChildren());
            return ret;
        }

        public virtual string classify(DataInstance instance)
        {
            string value_of_instance = instance.getProperty(this.label);

            Console.WriteLine($"Looking for my label, which is {this.label}, the instance has {value_of_instance}");
            // Check if we should pass this instance down to another node..
            foreach (Node child in this.getNodeChildren())
            {
                if (value_of_instance == child.value_splitter)
                {
                    return child.classify(instance);
                }else
                {
                    Console.WriteLine($"As {child.identifier}, I was looking for {child.value_splitter}");
                }
            }

            // Check if we should pass this instance down to one of our leafs.
            foreach(Leaf child in this.getLeafChildren())
            {
                if (value_of_instance == child.value_splitter)
                {
                    return child.classifier;
                }else
                {
                    Console.WriteLine($"As {child.identifier}, I was looking for {child.value_splitter}");
                }
            }

            // If we got here, the instance has a value for an attribute that this model does not know. ERROR TIME!
            throw new Exception($"Unknown value {value_of_instance} for {this.label}");
        }
    }
}
