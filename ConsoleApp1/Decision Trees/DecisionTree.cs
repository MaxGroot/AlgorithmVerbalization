using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DecisionTree
    {
        private Node root = null;
        private Node currentlySelectedNode = null;

        public DecisionTree addNode(string attribute, string value_splitter)
        {
            Node newnode = new Node(attribute, value_splitter);
            if (root == null)
            {
                root = newnode;
                currentlySelectedNode = root;
            } else
            {
                this.currentlySelectedNode.addChildNode(newnode);
                newnode.addParentNode(currentlySelectedNode);

                this.currentlySelectedNode = newnode;
            }
            return this;
        }

        public Leaf addLeaf(string value_splitter, string class_prediction)
        {
            Leaf leaf = new Leaf(value_splitter, class_prediction, this.currentlySelectedNode);
            currentlySelectedNode.addChildLeaf(leaf);

            return leaf;
        }

        public Leaf addBestGuessLeaf(string value_splitter, string class_prediction)
        {
            Leaf leaf = this.addLeaf(value_splitter, class_prediction);
            leaf.isBestGuess = true;
            return leaf;
        }

        public void moveSelectionUp()
        {
            this.currentlySelectedNode = this.currentlySelectedNode.getParent();
        }

        public Node getRoot()
        {
            return this.root;
        }
    }
}
