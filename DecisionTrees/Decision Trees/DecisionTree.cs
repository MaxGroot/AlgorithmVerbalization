using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    public class DecisionTree
    {
        private Node root = null;
        private int element_counter = 0;
        public Dictionary<Leaf, List<DataInstance>> data_locations;
        public string target_attribute;

        public DecisionTree(string target_attribute)
        {
            this.target_attribute = target_attribute;
            data_locations = new Dictionary<Leaf, List<DataInstance>>();
        }

        public Node addNode(string attribute, string value_splitter, Node parent, string element_identifier = null)
        {
            if (element_identifier == null)
            {
                element_identifier = ElementHelper.generateElementId('N',element_counter);
            }
            Node newnode = new Node(element_identifier,attribute, value_splitter);
            if (root == null)
            {
                root = newnode;
            } else
            {
                parent.addChildNode(newnode);
                newnode.addParentNode(parent);
            }
            element_counter++;
            return newnode;
        }
        public ContinuousNode addContinuousNode(string attribute, string value_splitter, double attribute_threshold, Node parent, string element_identifier = null)
        {
            if (element_identifier == null)
            {
                element_identifier = ElementHelper.generateElementId('N', element_counter);
            }
            ContinuousNode newnode = new ContinuousNode(element_identifier, attribute, value_splitter).setThreshold(attribute_threshold);
            if (root == null)
            {
                root = newnode;
            }
            else
            {
                parent.addChildNode(newnode);
                newnode.addParentNode(parent);
            }
            element_counter++;
            return newnode;
        }

        public Leaf addLeaf(string value_splitter, string class_prediction, Node parent, string element_identifier = null)
        {
            if (element_identifier == null)
            {
                element_identifier = ElementHelper.generateElementId('L', element_counter);
            }
            Leaf leaf = new Leaf(element_identifier, value_splitter, class_prediction, parent);
            parent.addChildLeaf(leaf);
            
            element_counter++;
            return leaf;
        }

        public Leaf addBestGuessLeaf(string value_splitter, string class_prediction, Node parent)
        {
            Leaf leaf = this.addLeaf(value_splitter, class_prediction, parent);
            leaf.certainty = -1;
            return leaf;
        }
        public Leaf addUncertainLeaf(string value_splitter, string class_prediction, Node parent, double certainty)
        {
            Leaf leaf = this.addLeaf(value_splitter, class_prediction, parent);
            leaf.certainty = certainty;
            return leaf;
        }

        public Node getRoot()
        {
            return this.root;
        }

        private void writeMyPosition(Node node)
        {
            string str = node.value_splitter;
            Node parent = node.getParent();
            while (parent != null)
            {
                str = parent.value_splitter + ", " + parent.label + " = " + str;
                parent = parent.getParent();
            }
            Console.WriteLine(str);
        }

        public DecisionTree fromNode(Node node)
        {
            this.root = node;
            return this;
        }

        public DecisionTree replaceNodeByNewLeaf(Node removeNode, bool check_parent_integrity = false)
        {
            // Create the new leaf
            List<DataInstance> total_set = new List<DataInstance>();
            List<Node> queue = new List<Node>();
            queue.Add(removeNode);

            // Get all instances that should be covered.
            while (queue.Count > 0)
            {
                Node node = queue[0];
                queue.RemoveAt(0);

                foreach (Leaf child in node.getLeafChildren())
                {
                    total_set.AddRange(this.data_locations[child]);
                    this.data_locations.Remove(child);
                }

                // Add child nodes to queue so their leafs also get added
                queue.AddRange(node.getNodeChildren());
            }

            Node parent = removeNode.getParent();
            // Remove the old node from its parent.
            if (parent != null)
            {
                // Make the new leaf
                string prediction = SetHelper.mostCommonClassifier(total_set, this.target_attribute);
                double uncertainty = (double)SetHelper.subset_errors(total_set, this.target_attribute) / (double)total_set.Count;
                Leaf newleaf = this.addUncertainLeaf(removeNode.value_splitter, prediction, parent, uncertainty);
                // Make sure we can access this leaf's new subset!
                this.data_locations[newleaf] = total_set;

                parent.removeChildNode(removeNode);

                // Creating a leaf can create the possibility of a node losing its integrity (i.e. it has leafs with the same classifier)
                // If we need to check for that, call the integrity check method. 
                if (check_parent_integrity)
                {
                    this.verifyNodeIntegrity(parent);
                }
            }
            return this;
        }
        
        public DecisionTree verifyIntegrity()
        {
            List<Node> bottom_up_nodes = this.sort_nodes_bottom_up();
            foreach(Node node in bottom_up_nodes)
            {
                this.verifyNodeIntegrity(node);
            }

            return this;
        }

        public List<Node> sort_nodes_bottom_up()
        {
            // Find all nodes that have at least 1 leaf child, as they might be up for consideration of pruning.
            Dictionary<string, Node> node_queue_with_identifiers = new Dictionary<string, Node>();
            foreach (Leaf leaf in this.data_locations.Keys.ToList())
            {
                // We need to check for unique nodes, therefore we work with a dictionary to prevent a node occuring multiple times.
                if (!node_queue_with_identifiers.ContainsKey(leaf.parent.identifier))
                {
                    // This node has not yet been added, so add it now.
                    node_queue_with_identifiers.Add(leaf.parent.identifier, leaf.parent);
                }
            }

            List<Node> nodes =  node_queue_with_identifiers.Values.ToList();
            

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
                    return -pair1.Value.CompareTo(pair2.Value);
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

        private DecisionTree verifyNodeIntegrity(Node node)
        {

            return this;
        }

        public string classify(DataInstance instance)
        {
            return this.getRoot().classify(instance);
        }
    }
}
