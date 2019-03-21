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
        private int element_counter = 0;
        public Node addNode(string attribute, string value_splitter, Node parent, string element_identifier = null)
        {
            if (element_identifier == null)
            {
                element_identifier = Calculator.generateElementId('T',element_counter);
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
                element_identifier = Calculator.generateElementId('T', element_counter);
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
                element_identifier = Calculator.generateElementId('T', element_counter);
            }
            Leaf leaf = new Leaf(element_identifier, value_splitter, class_prediction, parent);
            parent.addChildLeaf(leaf);

            element_counter++;
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


        public DataInstance classify(DataInstance instance, string classifier_name)
        {
            string classifier =  this.getRoot().classify(instance);
            instance = instance.overwriteProperty(classifier_name, classifier);
            return instance;
        }
    }
}
