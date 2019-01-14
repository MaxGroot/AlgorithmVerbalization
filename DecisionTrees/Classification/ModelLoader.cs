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
        public DecisionTree load_model(string model_location)
        {
            DecisionTree model = new DecisionTree();
            var reader = new StreamReader(model_location);
            Node last_node = null;

            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                update_model_with_line(ref model, ref last_node, line);
            }

            return model;
        }

        private void update_model_with_line(ref DecisionTree model, ref Node previous_node, string line)
        {
            Console.WriteLine(line);
            string[] split = line.Split('-');

            int level = int.Parse(split[0]);
            // We know that our parent is the first node we encounter on the previous level, since we do depth-first save.
            int wanted_level = level - 1;

            string type = split[1];
            string label = split[2];
            string value_split = split[3];
            
            if (type == "NODE")
            {
                if (previous_node != null)
                {
                    while (node_level(previous_node) > wanted_level)
                    {
                        previous_node = previous_node.getParent();
                        if (previous_node == null)
                        {
                            throw new Exception($"No parent found for {line}");
                        }
                    }
                }

                Node node = model.addNode(label, value_split, previous_node);
                previous_node = node;
            } else if (type == "LEAF")
            {
                string leaf_classifier = split[4];
                string leaf_type = split[5];

                if (leaf_type=="ESTIMATE")
                {
                    model.addBestGuessLeaf(value_split, leaf_classifier, previous_node);
                }else if (leaf_type=="PERFECT")
                {
                    model.addLeaf(value_split, leaf_classifier, previous_node);
                }
                else
                {
                    throw new Exception($"UNKNOWN LEAF TYPE:{leaf_type}");
                }
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
