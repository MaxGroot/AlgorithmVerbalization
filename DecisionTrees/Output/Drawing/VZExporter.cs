using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class VZExporter
    {
        public List<string> lines(DecisionTree tree)
        {
            List<string> output_lines = new List<string>();

            output_lines.Add("digraph DecisionTree {");
            output_lines.Add("// Nodes and leafs");

            List<string> element_lines = new List<string>();
            List<string> connection_lines = new List<string>();

            List<Node> queue = new List<Node>();
            queue.Add(tree.getRoot());

            while (queue.Count > 0)
            {
                iterate(ref queue, ref element_lines, ref connection_lines);
            }

            output_lines.AddRange(element_lines);
            output_lines.Add("// Connections");
            output_lines.AddRange(connection_lines);

            output_lines.Add("}");

            return output_lines;
        }

        private void iterate(ref List<Node> queue, ref List<string> element_lines, ref List<string> connection_lines)
        {
            Node node = queue[0];
            queue.RemoveAt(0);
            element_lines.Add(nodeToLine(node));

            foreach (Leaf child in node.getLeafChildren())
            {
                element_lines.Add(leafToLine(child));
            }

            connection_lines.AddRange(nodeConnections(node));
            foreach (Node child in node.getNodeChildren())
            {
                queue.Add(child);
            }
        }

        private string nodeToLine(Node node)
        {
            return $"N{node.identifier} [shape=box, label=\"{node.label}\"];";
        }
        private List<string> nodeConnections(Node node)
        {
            List<string> new_lines = new List<string>();

            string add_threshold = "";
            ContinuousNode cNode = null;
            if (node is ContinuousNode)
            {
                cNode = (ContinuousNode)node;
            }
            if (cNode != null)
            {
                add_threshold = $"{cNode.threshold}";
            }
            foreach (Node child in node.getNodeChildren())
            {
                new_lines.Add($"N{node.identifier}->N{child.identifier} [label=\"{child.value_splitter} {add_threshold} \"];");
            }
            foreach (Leaf child in node.getLeafChildren())
            {
                new_lines.Add($"N{node.identifier}->L{child.identifier} [label=\"{child.value_splitter} {add_threshold} \"];");
            }

            return new_lines;
        }
        private string leafToLine(Leaf leaf)
        {
            return $"L{leaf.identifier} [label=\"{leaf.classifier}\"];";
        }
    }
}
