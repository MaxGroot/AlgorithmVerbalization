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

        public Node addNode(string attribute, string value_splitter, Node parent)
        {
            Node newnode = new Node(attribute, value_splitter);
            if (root == null)
            {
                root = newnode;
            } else
            {
                parent.addChildNode(newnode);
                newnode.addParentNode(parent);
            }
            return newnode;
        }

        public Leaf addLeaf(string value_splitter, string class_prediction, Node parent)
        {
            Leaf leaf = new Leaf(value_splitter, class_prediction, parent);
            parent.addChildLeaf(leaf);

            return leaf;
        }

        public Leaf addBestGuessLeaf(string value_splitter, string class_prediction, Node parent)
        {
            Leaf leaf = this.addLeaf(value_splitter, class_prediction, parent);
            leaf.isBestGuess = true;
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
    }
}
