using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class DecisionTree
    {
        private Node root = null;
        private Node currentlySelectedNode = null;

        public DecisionTree addNode(string attribute)
        {
            if (root == null)
            {
                root = new Node(attribute);
                currentlySelectedNode = root;
            } else
            {
                Node newnode = new Node(attribute);

                this.currentlySelectedNode.addChildNode(newnode);
                newnode.addParentNode(currentlySelectedNode);

                this.currentlySelectedNode = newnode;
            }
            return this;
        }

        public DecisionTree addLeaf(string value_splitter, string class_prediction)
        {
            Leaf leaf = new Leaf(value_splitter, class_prediction, this.currentlySelectedNode);
            return this;
        }
    }
}
