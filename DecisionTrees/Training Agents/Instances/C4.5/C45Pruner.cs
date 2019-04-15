using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class C45Pruner
    {
        private Dictionary<string, double> calculated_upperBounds = new Dictionary<string, double>();


        // 25 * 2
        private int confidence = 50;
        private Dictionary<Leaf, List<DataInstance>> data_locations;

        public DecisionTree prune(DecisionTree tree, string target_attribute, Dictionary<Leaf, List<DataInstance>> data_locations)
        {
            this.data_locations = data_locations;


            List<Node> nodes_unsorted = this.nodes_with_leafs(this.data_locations.Keys.ToList());
            List<Node> queue = this.sort_nodes_bottom_up(nodes_unsorted);

            // Start post-pruning with this queue.
            return pruneIterate(tree, queue, target_attribute);

        }

        private DecisionTree pruneIterate(DecisionTree tree, List<Node> queue, string target_attribute)
        {
            // Manage queue.
            Node node = queue[0];
            queue.RemoveAt(0);


            // Lets consider this node.
            Console.WriteLine($"{node.identifier} under consideration. ");
            List<DataInstance> node_set = new List<DataInstance>();

            // Calculate error estimate of the leafs

            double leaf_errors = 0;
            foreach(Leaf child in node.getLeafChildren())
            {
                List<DataInstance> leaf_set = this.data_locations[child];
                node_set.AddRange(leaf_set);


                // Calculate estimated error.
                int errors = SetHelper.subset_errors(leaf_set, target_attribute);
                double errorRate = this.calcErrorRate(errors, leaf_set.Count);
                double estimatedError = errorRate * leaf_set.Count;
                leaf_errors += estimatedError;
            }

            // Calculate estimated error of node.
            int node_errors = SetHelper.subset_errors(node_set, target_attribute);
            double nodeErrorRate = this.calcErrorRate(node_errors, node_set.Count);
            double nodeEstimatedError = nodeErrorRate * node_set.Count;

            // Compare
            Console.WriteLine($"Node error rate: {nodeEstimatedError}. Leaf estimated error: {leaf_errors}. Prune: {nodeEstimatedError < leaf_errors}");
            if (nodeEstimatedError < leaf_errors)
            {
                Console.WriteLine("PRUNE!");
                // We need to prune!
                tree = this.replaceNodeByNewLeaf(tree, node, node_set, target_attribute);
            }

            // Iterate further if necessary. 
            if (queue.Count > 0)
            {
                tree = this.pruneIterate(tree, queue, target_attribute);
            }
            return tree;
            
        }
        private List<Node> nodes_with_leafs(List<Leaf> leafs)
        {
            // Find all nodes that have at least 1 leaf child, as they might be up for consideration of pruning.
            Dictionary<string, Node> node_queue_with_identifiers = new Dictionary<string, Node>();
            foreach (Leaf leaf in leafs)
            {
                if (!node_queue_with_identifiers.ContainsKey(leaf.parent.identifier))
                {
                    // This node has not yet been added, so add it now.
                    node_queue_with_identifiers.Add(leaf.parent.identifier, leaf.parent);
                }
            }

            return node_queue_with_identifiers.Values.ToList();
        }
        private List<Node> sort_nodes_bottom_up(List<Node> nodes)
        {

            // We now have a queue of nodes that have at least 1 leaf child. 
            // However, we need to sort it such that we will go through it bottom-up.
            // First, convert it to a dictionary containing the parent counts.

            Dictionary<Node, int> node_queue_with_parentcounts = new Dictionary<Node, int>();
            foreach (Node node in nodes)
            {
                node_queue_with_parentcounts[node] = ElementHelper.parentCount(node);
            }

            // Then, convert it to a sorted dictionary, descending by parent count to ensure bottom-up.
            List<KeyValuePair<Node, int>> sortedNodes = node_queue_with_parentcounts.ToList();
            sortedNodes.Sort(
                delegate (KeyValuePair<Node, int> pair1,
                KeyValuePair<Node, int> pair2)
                {
                    return - pair1.Value.CompareTo(pair2.Value);
                }
            );

            // Make a queue out of the sorted dictionary.
            List<Node> queue = new List<Node>();
            foreach (KeyValuePair<Node, int> row in sortedNodes)
            {
                queue.Add(row.Key);
            }

            return queue;
        }
        private double calcErrorRate(int successes, int sampleSize)
        {
            string calculationKey = $"{successes},{sampleSize}";
            if (! calculated_upperBounds.ContainsKey(calculationKey))
            {
                double errorRate = Calculator.upperBoundGood(successes, sampleSize, this.confidence);
                calculated_upperBounds[calculationKey] = errorRate;
            }

            return calculated_upperBounds[calculationKey];
            
        }
        private DecisionTree replaceNodeByNewLeaf(DecisionTree tree, Node node, List<DataInstance> node_set, string target_attribute)
        {
            // Create the new leaf
            string prediction = SetHelper.mostCommonClassifier(node_set, target_attribute);
            double uncertainty = (double) SetHelper.subset_errors(node_set, target_attribute) / (double) node_set.Count;

            Node parent = node.getParent();
            Leaf newleaf = tree.addUncertainLeaf(node.value_splitter, prediction, parent, uncertainty);

            // Make sure we can access this leaf's new subset!
            data_locations[newleaf] = node_set;

            // Remove the old node from its parent.
            if (parent != null)
            {
                parent.removeChildNode(node);
            }

            return tree;
        }
    }
}
