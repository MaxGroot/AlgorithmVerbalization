using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    public class Leaf: ITreeElement
    {
        public string identifier;
        public string value_splitter;
        public string classifier;
        public Node parent;
        public double certainty; 

        public Leaf(string identifier, string value_splitter, string classifier, Node parent)
        {
            this.identifier = identifier;
            this.value_splitter = value_splitter;
            this.classifier = classifier;
            this.parent = parent;
            this.certainty = 1;
        }

        public string line()
        {
            return this.value_splitter;
        }
        public string underline()
        {
            return this.classifier;
        }
        public List<Node> nodechildren()
        {
            return new List<Node>();
        }
        public List<Leaf> leafchildren()
        {
            return new List<Leaf>();
        }

        public string rule()
        {
            string rule = "";
            if (this.parent != null)
            {
                ContinuousNode cNode = null;
                if (this.parent is ContinuousNode)
                {
                    cNode = (ContinuousNode)this.parent;
                    rule = this.value_splitter + cNode.threshold.ToString() + " : ";
                }
                else
                {
                    rule = this.value_splitter + " : ";
                }
            }
            return rule + this.classifier;
        }
    }
}
