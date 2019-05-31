using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DecisionTrees
{
    class ModelLoader
    {
        private Dictionary<string, Node> node_identifiers = new Dictionary<string, Node>();

        public DecisionTree load_model(string model_location, string target_attribute)
        {
            DecisionTree model = new DecisionTree(target_attribute);
            var reader = new StreamReader(model_location);
            
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                update_model_with_line(ref model, line);
            }
            return model;
        }

        private void update_model_with_line(ref DecisionTree model, string line)
        {
            string[] split = line.Split('|');
            string type = split[0];
            string identifier = split[1];
            string parentidentifier = split[2];
            Node parent = null;
            if (this.node_identifiers.ContainsKey(parentidentifier))
            {
                parent = this.node_identifiers[parentidentifier];
            }
            else
            {
                // We could not find the parent for this element. Maybe its the root element? If so, we can accept parent equalling null.
                if (parentidentifier != "ROOT")
                {
                    // Its not root! Throw an exception.
                    throw new Exception($"Could not find parent {parentidentifier} of {identifier}");
                }
            }
            if (type == "NODE")
            {
                string typeOfNode = split[3];
                string label = split[4];
                string value_split = split[5];
                Node node = null;
                if (typeOfNode == "C")
                {
                    // Continuous node
                    string threshold_string = split[6];
                    double threshold = double.Parse(threshold_string);
                    node = model.addContinuousNode(label, value_split, threshold, parent);
                }else
                {
                    // Nominal node
                    node = model.addNode(label, value_split, parent);
                }
                node.identifier = identifier;
                this.node_identifiers[identifier] = node;

            } else if (type == "LEAF")
            {
                string leaf_value_splitter = split[3];
                string leaf_classifier = split[4];
                double leaf_certainty = double.Parse(split[5]);
                
                Leaf leaf = model.addUncertainLeaf(leaf_value_splitter, leaf_classifier, parent, leaf_certainty);
                leaf.identifier = identifier;
            } else
            {
                throw new Exception($"UNKNOWN TREE ELEMENT:{type}");
            }
        }
        
        private static int node_level(Node node)
        {
            int node_level = 0;
            if (node == null)
            {
                return 0;
            }
            while (true)
            {
                if (node.getParent() == null)
                {
                    break;
                }
                else
                {
                    node_level++;
                    node = node.getParent();
                }
            }
            return node_level;
        }
    }
}
