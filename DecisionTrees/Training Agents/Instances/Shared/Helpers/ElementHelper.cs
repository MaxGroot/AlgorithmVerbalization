using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ElementHelper
    {
        private static List<string> alphabet()
        {
            return new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }


        public static string generateElementId(char character, int counter)
        {
            int second_letter_count = counter % 26;
            int first_letter_count = (int)counter / 26;

            return character.ToString() + "_" + alphabet()[first_letter_count] + alphabet()[second_letter_count];
        }

        public static int parentCount(Node node)
        {
            int i = 0;
            while(true)
            {
                if (node.getParent() != null)
                {
                    node = node.getParent();
                    i++;
                }else
                {
                    break;
                }
            }
            return i;
        }

        public static DecisionTree nodeAsTree(DecisionTree originaltree, Node node)
        {
            DecisionTree newtree = new DecisionTree("UNKNOWN");
            newtree = newtree.fromNode(node);

            List<Node> queue = new List<Node>();
            queue.Add(node);

            while(queue.Count > 0)
            {
                Node check = queue[0];
                queue.RemoveAt(0);

                // Add children nodes to queue
                queue.AddRange(check.getNodeChildren());

                // Add data locations of my leafs
                foreach(Leaf leaf in check.getLeafChildren())
                {
                    newtree.data_locations[leaf] = originaltree.data_locations[leaf];    
                }
            }

            return newtree;
        }
    }
}
