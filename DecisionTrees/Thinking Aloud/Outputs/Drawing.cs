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
        public Drawing(DecisionTree tree)
        {
            this.tree = tree;
        }

        public List<string> lines()
        {
            List<string> lines = new List<string>();
            List<DrawElement> all_elements = new List<DrawElement>();

            DrawElement[,] raster = new DrawElement[,] { };

            List<DrawElement> queue = new List<DrawElement>();

            // Create the root
            DrawElement root_element = new DrawElement(tree.getRoot(), 'N', 0, 0);
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
                Console.WriteLine($"({el.x}, {el.y})"); 
            }


            return lines;
        }

        private void iterate(ref List<DrawElement> queue, ref DrawElement[,] raster, ref List<DrawElement> all_elements)
        {
            // Manage queue
            Node node = queue[0].node;
            int currentX = queue[0].x;
            int currentY = queue[0].y;

            queue.RemoveAt(0);

            Console.WriteLine($"ITERATE ON {node.label}, starting from {currentX}, {currentY}");

            // LETS MAKE BABIES. 
            int children_count = node.getNodeChildren().Count;
            int side_offset = (int)Math.Floor( (double) (children_count / 2));

            List<DrawElement> new_babies = new List<DrawElement>();
            int i = 0;
            foreach(Node baby in node.getNodeChildren())
            {
                int x = currentX;
                x += (i - side_offset);

                int y = currentY + 1;

                Console.WriteLine($"New baby {baby.label} on {x},{y}");
                DrawElement el = new DrawElement(baby, 'N', x, y);
                new_babies.Add(el);

                i++;
            }

            // Babies are made, lets now move up existing drawelements.
            Console.WriteLine($"Move with {side_offset} from {currentX}"); 
            move_other_elements(ref all_elements, currentX, side_offset);

            // Add the babies to the all elements
            all_elements.AddRange(new_babies);
            queue.AddRange(new_babies);
        }

        private void move_other_elements(ref List<DrawElement> all_elements, int x_threshold, int side_offset)
        {
            foreach(DrawElement el in all_elements)
            {
                if (el.x < x_threshold)
                {
                    el.x -= side_offset;
                }
                if (el.x > x_threshold)
                {
                    Console.WriteLine("This happened");
                    el.x += side_offset;
                }
            }
        }
        
    }
}
