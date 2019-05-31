using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ModelManager
    {
        public static List<string> generate_model(DecisionTree tree)
        {
            List<string> model_lines = new List<string>();
            List<Node> queue = new List<Node>();
            queue.Add(tree.getRoot());

            while(queue.Count > 0)
            {
                iterate_model(ref queue, ref model_lines);
            }

            return model_lines;
        }
        public static List<string> generate_rules(DecisionTree tree)
        {
            List<string> rules = new List<string>();
            List<Node> queue = new List<Node>();
            queue.Add(tree.getRoot());
            
            while(queue.Count > 0)
            {
                iterate_rules(ref queue, ref rules);
            }

            return rules;
        }

        private static void iterate_rules(ref List<Node> queue, ref List<string> rule_lines)
        {
            Node node = queue[0];
            queue.RemoveAt(0);
            int tabs = node_level(node);
            rule_lines.AddRange(nodeToRuleLines(node));

            int i = 0;
            // We want to save the rules depth-first. Therefore, we add the node children to the front of the queue.
            // We keep tally with i to make sure the children are not all added to the front because that would insert them in reversed order.
            foreach(Node child in node.getNodeChildren())
            {
                queue.Insert(i, child);
                i++;
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

        private static List<string> nodeToRuleLines(Node node)
        {
            List<string> lines = new List<string>();
            int amountOfTabs = node_level(node);
            string tabs = new string('\t', amountOfTabs);

            lines.Add(tabs + node.rule());

            foreach(Leaf child in node.getLeafChildren())
            {
                lines.Add(new string('\t', amountOfTabs + 1) + child.rule());
            }

            return lines;
            
        }

        private static void iterate_model(ref List<Node> queue, ref List<string> outputs)
        {
            Node node = queue[0];
            queue.RemoveAt(0);
            outputs.Add(nodeToModelLine(node, "|"));
            foreach (Node child in node.getNodeChildren())
            {
                queue.Add(child);
            }
            foreach(Leaf child in node.getLeafChildren())
            {
                outputs.Add(leafToModelLine(child, "|"));
            }
        }

        private static string nodeToModelLine(Node node, string sep)
        {
            ContinuousNode cNode = null;
            if (node is ContinuousNode)
            {
                cNode = (ContinuousNode) node;
            }
            string parentidentifier = (node.getParent() == null) ? "ROOT" : node.getParent().identifier;
            return $"NODE{sep}{node.identifier}{sep}{parentidentifier}{sep}{(node is ContinuousNode ? 'C' : 'N')}{sep}{node.label}{sep}{node.value_splitter}{(node is ContinuousNode ? $"{sep}{cNode.threshold}" : "")}";
        }

        private static string leafToModelLine(Leaf leaf, string sep)
        {
            string classifying_strength = leaf.certainty.ToString();
            return $"LEAF{sep}{leaf.identifier}{sep}{leaf.parent.identifier}{sep}{leaf.value_splitter}{sep}{leaf.classifier}{sep}{classifying_strength}";
        }
    }
}
