using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ModelManager
    {
        public static List<string> output(DecisionTree tree)
        {
            List<string> output_lines = new List<string>();
            List<Node> queue = new List<Node>();
            queue.Add(tree.getRoot());

            while(queue.Count > 0)
            {
                iterate(ref queue, ref output_lines);
            }

            return output_lines;
        }


        private static void iterate(ref List<Node> queue, ref List<string> outputs)
        {
            Node node = queue[0];
            queue.RemoveAt(0);
            outputs.Add(nodeToLine(node));
            foreach (Node child in node.getNodeChildren())
            {
                queue.Add(child);
            }
            foreach(Leaf child in node.getLeafChildren())
            {
                outputs.Add(leafToLine(child));
            }
        }

        private static int node_level(Node node)
        {
            int node_level = 0;
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

        private static string nodeToLine(Node node)
        {
            int level = node_level(node);
            ContinuousNode cNode = null;
            if (node is ContinuousNode)
            {
                cNode = (ContinuousNode) node;
            }
            return $"{level}-NODE-{(node is ContinuousNode ? 'C' : 'N')}-{node.identifier}-{node.label}-{node.value_splitter}{(node is ContinuousNode ? $"-{cNode.threshold}" : "")}";
        }
        private static string leafToLine(Leaf leaf)
        {
            int level = node_level(leaf.parent) + 1;
            string classifying_strength = leaf.certainty.ToString();
            return $"{level}-LEAF-{leaf.identifier}-{leaf.value_splitter}-{leaf.classifier}-{classifying_strength}";
        }
    }
}
