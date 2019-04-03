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

        public string myRule(string targetAttribute)
        {
            string rule = this.parent.triggerRule(value_splitter);
            
            rule += " THEN " + targetAttribute + " = " + this.classifier;
            return rule;
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
    }
}
