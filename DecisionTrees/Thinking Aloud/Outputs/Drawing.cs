using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class Drawing
    {
        private DecisionTree tree;
        private int el_counter;
        public Drawing(DecisionTree tree)
        {
            this.tree = tree;
            el_counter = 0;
        }

        public List<string> lines()
        {
            List<string> lines = new List<string>();
            List<DrawElement> all_elements = new List<DrawElement>();

            DrawElement[,] raster = new DrawElement[,] { };

            List<DrawElement> queue = new List<DrawElement>();

            // Create the root
            DrawElement root_element = new DrawElement(tree.getRoot(), "0", 0, 0);
            all_elements.Add(root_element);

            // Add the root's kids to the queue.
            queue.Add(root_element);


            // Iterate!
            while (queue.Count > 0)
            {
                iterate(ref queue, ref raster, ref all_elements);
            }

            foreach(DrawElement el in all_elements)
            {
                Console.WriteLine($"({el.x}, {el.y}) : {el.label}"); 
            }


            return lines;
        }

        private void iterate(ref List<DrawElement> queue, ref DrawElement[,] raster, ref List<DrawElement> all_elements)
        {
            // Manage queue
            Node node = queue[0].node;
            int currentX = queue[0].x;
            int currentY = queue[0].y;

            Console.WriteLine($"Iterate on {node.label}, Y= {currentY}, X= {currentX}");
            queue.RemoveAt(0);

            // LETS MAKE BABIES. 
            // First, find out some global stuff
            int node_children_count = node.getNodeChildren().Count;
            int leaf_children_count = node.getLeafChildren().Count;
            int children_count = node_children_count + leaf_children_count;

            int side_offset = (int)Math.Floor( (double) (children_count / 2));

            List<DrawElement> node_babies = new List<DrawElement>();
            List<DrawElement> leaf_babies = new List<DrawElement>();

            int i = 0;
            int character_width = 0;
            foreach(Node baby in node.getNodeChildren())
            {
                el_counter++;
                int x = currentX;
                x += (i - side_offset);

                int y = currentY + 1;

                Console.WriteLine($"Nodebaby on {x} , {y}: {el_counter}");
                DrawElement el = new DrawElement(baby, $"N{el_counter}", x, y);
                node_babies.Add(el);

                i++;
            }
            foreach(Leaf baby in node.getLeafChildren())
            {
                el_counter++;
                int x = currentX;
                x += (i - side_offset);
                int y = currentY + 1;


                Console.WriteLine($"Leafbaby on {x}, {y}: {el_counter}");
                DrawElement el = new DrawElement(null, $"L{el_counter}", x, y);
                leaf_babies.Add(el);

                i++;
            }

            // Babies are made, lets now move up existing drawelements.
            Console.WriteLine($"Move from {currentX} with {side_offset}");
            move_other_elements(ref all_elements, currentX, side_offset);

            // Add the babies to the all elements
            all_elements.AddRange(node_babies);
            all_elements.AddRange(leaf_babies);

            queue.AddRange(node_babies);
        }

        private void move_other_elements(ref List<DrawElement> all_elements, int x_threshold, int side_offset)
        {
            foreach(DrawElement el in all_elements)
            {
                if (el.x < x_threshold)
                {
                    el.x -= side_offset;
                    Console.WriteLine($"Moved {el.label} left");
                }
                if (el.x > x_threshold)
                {
                    el.x += side_offset;
                    Console.WriteLine($"Moved {el.label} right");
                }
            }
        }
        
    }
}
