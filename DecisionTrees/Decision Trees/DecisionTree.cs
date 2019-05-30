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

        public DecisionTree replaceNodeByNewLeaf(Node removeNode)
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
                
            }
            return this;
        }
        
        public DecisionTree verifyIntegrity()
        {
            List<Node> bottom_up_nodes = this.sort_nodes_bottom_up();
            while(bottom_up_nodes.Count > 0)
            {
                Node node = bottom_up_nodes[0];
                bottom_up_nodes.RemoveAt(0);
                if (node.isDeleted())
                {
                    // This node was deleted before we arrived at it from the queue and therefore we shouldnt analyze it.
                }
                else
                {
                    if (this.verifyNodeIntegrity(node))
                    {
                        // That node was removed! We need to add this node's parent to the end of the queue
                        bottom_up_nodes.Add(node.getParent());
                    }
                }
            }

            return this;
        }

        public List<Node> sort_nodes_bottom_up()
        {
            // Find all nodes that have at least 1 leaf child, as they might be up for consideration.
            List<Node> discover_queue = new List<Node>();
            List<Node> nodes = new List<Node>();
            discover_queue.Add(this.getRoot());

            while(discover_queue.Count > 0)
            {
                Node node = discover_queue[0];
                discover_queue.RemoveAt(0);
                discover_queue.AddRange(node.getNodeChildren());

                if (node.getLeafChildren().Count > 0)
                {
                    nodes.Add(node);
                }
            }

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

        private bool verifyNodeIntegrity(Node node)
        {
            bool should_remove_node = false;
            if (node is ContinuousNode)
            {
                string first_classifier = null;
                foreach(Leaf child in node.getLeafChildren())
                {
                   if (child.classifier != first_classifier)
                    {
                        first_classifier = child.classifier;
                    }else
                    {
                        should_remove_node = true;
                    }
                }
                if (should_remove_node)
                {
                    // First we need to combine the two leafs of this node into one. We'll stick the latter in the former.
                    Leaf child_to_remove = node.getLeafChildren().Last();
                    Leaf child_to_keep = node.getLeafChildren().First();

                    // Move the data
                    this.data_locations[child_to_keep].AddRange(data_locations[child_to_remove]);
                    this.data_locations.Remove(child_to_remove);

                    // Remove the removal child from this node.
                    node.removeChildLeaf(child_to_remove);

                    // We now have a node with only one leaf. That shouldnt happen. But it will be addressed outside of this if condition!
                }
            }
            else
            {
                // Check if this is a nominal node that results in all the same classifier

                // If it has at least one node as a child, we do not want to remove it
                should_remove_node = (node.getNodeChildren().Count == 0);
                string first_classifier = null;
                foreach(Leaf child in node.getLeafChildren())
                {
                    if (first_classifier == null)
                    {
                        first_classifier = child.classifier;
                    }
                    if (child.classifier != first_classifier)
                    {
                        should_remove_node = false;
                    }
                }
            }

            if (should_remove_node)
            {
                Node parent = node.getParent();
                if (parent == null)
                {
                    throw new Exception("Tried to remove the root of the DecisionTree!");
                }
                // This node is useless. We need to stick its leaf to the parent on this node's value splitter.
                Leaf my_child = null;
                if (node.getLeafChildren().Count == 1)
                {
                    my_child = node.getLeafChildren().First();

                    my_child.value_splitter = node.value_splitter;

                    // Attach the child to this node's parent, remove this node from its parent
                    parent.addChildLeaf(my_child);

                }
                else
                {
                    string classifier = node.getLeafChildren().First().classifier;
                    List<DataInstance> full_dataset = new List<DataInstance>();

                    double average_certainty = 0;

                    // Relocate the data.
                    foreach(Leaf child in node.getLeafChildren())
                    {
                        full_dataset.AddRange(data_locations[child]);
                        data_locations.Remove(child);
                        average_certainty += child.certainty;
                    }
                    average_certainty /= (double)node.getLeafChildren().Count;

                    // We need to merge all leafs into one.
                    my_child = this.addUncertainLeaf(node.value_splitter, classifier, parent, average_certainty);
                    data_locations[my_child] = full_dataset;
                }

                // Now that all leafs have been dealt with, we need to delete the node
                parent.removeChildNode(node);
                node.delete();
            }

            return should_remove_node;
        }

        public string classify(DataInstance instance)
        {
            return this.getRoot().classify(instance);
        }
    }
}
