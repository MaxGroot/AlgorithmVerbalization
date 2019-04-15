using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DotExport
    {
        private int total_set_count;
        private DecisionTree tree;

        public List<string> lines(DecisionTree input_tree)
        {
            this.tree = input_tree;

            List<string> output_lines = new List<string>();

            this.total_set_count = SetHelper.total_subset_of_data_locations(tree.data_locations).Count;

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
            return $"N{node.identifier} [shape=box, label=\"{node.label} \n [{node.identifier}]\"];";
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
            return $"L{leaf.identifier} [shape=underline, label=\"{leaf.classifier} \n {tree.data_locations[leaf].Count} / {total_set_count} ({Math.Round(leaf.certainty, 2)}) \n [{leaf.identifier}]\", color=blue];";
        }
    }
}
