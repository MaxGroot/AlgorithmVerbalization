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

            // Make the new leaf
            string prediction = SetHelper.mostCommonClassifier(total_set, this.target_attribute);
            double uncertainty = (double)SetHelper.subset_errors(total_set, this.target_attribute) / (double)total_set.Count;
            Node parent = removeNode.getParent();
            Leaf newleaf = this.addUncertainLeaf(removeNode.value_splitter, prediction, parent, uncertainty);

            // Make sure we can access this leaf's new subset!
            this.data_locations[newleaf] = total_set;

            // Remove the old node from its parent.
            if (parent != null)
            {
                parent.removeChildNode(removeNode);
                if (check_parent_integrity)
                {
                    this.verifyNodeIntegrity(parent);
                }
            }
            return this;
        }
        
        public DecisionTree verifyNodeIntegrity(Node node)
        {
            Dictionary<string, Leaf> classifier_leafs = new Dictionary<string, Leaf>();
            List<Leaf> children_to_remove = new List<Leaf>();

            foreach (Leaf child in node.getLeafChildren())
            {
                if (!classifier_leafs.ContainsKey(child.classifier))
                {
                    classifier_leafs[child.classifier] = child;
                }
                else
                {
                    // We already have this classifier for this node! Merge this child in the other
                    if (!(node is ContinuousNode))
                    {
                        children_to_remove.Add(child);
                    }else
                    {
                        // This is a continuous node so we should remove the node anyway since that's always a binary split
                        return this.replaceNodeByNewLeaf(node, true);
                    }
                }
            }

            // If we got here, then this is a nominal node that possible requires shifting leafs arround.

            foreach (Leaf child in children_to_remove)
            {
                // Relocate the data instances from this child to the first child that had this classifier
                this.data_locations[classifier_leafs[child.classifier]].AddRange(this.data_locations[child]);
                child.classifier = "SHOULD BE REMOVED";
                child.value_splitter = "SHOULD HAVE BEEN REMOVED";
                node.removeChildLeaf(child);
            }
            return this;
        }
        public string classify(DataInstance instance)
        {
            return this.getRoot().classify(instance);
        }
    }
}
