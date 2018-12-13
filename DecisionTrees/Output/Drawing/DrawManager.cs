using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DrawManager
    {
        private DecisionTree tree;
        private int el_counter;
        private int lowest_x, highest_x, highest_y;
        public DrawManager(DecisionTree tree)
        {
            this.tree = tree;
            el_counter = 0;
            lowest_x = 0;
            highest_x = 0;
            highest_y = 0;
        }

        public List<string> lines()
        {
            List<string> lines = new List<string>();
            List<DrawElement> all_elements = new List<DrawElement>();

            List<DrawElement> queue = new List<DrawElement>();

            // Create the root
            DrawElement root_element = new DrawElement(tree.getRoot(), "R", 0, 0);
            all_elements.Add(root_element);

            // Add the root's kids to the queue.
            queue.Add(root_element);


            // Iterate!
            while (queue.Count > 0)
            {
                iterate(ref queue, ref all_elements);
            }
            foreach(DrawElement el in all_elements)
            {
                el.x += Math.Abs(this.lowest_x);
            }
            highest_x += Math.Abs(lowest_x);
            // Each element now knows it's own position. They are placed in a minimal raster that starts at 0,0. 
            // We loop through them and write it in lines.
            for(int j=0; j<=highest_y; j+=1)
            {
                lines.Add(new string(' ', highest_x+1));
            }
            foreach (DrawElement el in all_elements)
            {
                StringBuilder b = new StringBuilder(lines[el.y]);
                b[el.x] = el.label[0];
                lines[el.y] = b.ToString();
            }


            return lines;
        }

        private void iterate(ref List<DrawElement> queue, ref List<DrawElement> all_elements)
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
            foreach(Node baby in node.getNodeChildren())
            {
                el_counter++;
                int x = currentX;
                x += (i - side_offset);
                
                int y = currentY + 1;

                Console.WriteLine($"Nodebaby on {x} , {y}: {el_counter}");
                DrawElement el = new DrawElement(baby, $"N{el_counter}", x, y);

                update_coordinate_variables(el.x, el.y);

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

                update_coordinate_variables(el.x, el.y);

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
                    update_coordinate_variables(el.x, el.y);
                }
                if (el.x > x_threshold)
                {
                    el.x += side_offset;
                    update_coordinate_variables(el.x, el.y);
                }
            }
        }

        private void update_coordinate_variables(int x, int y)
        {
            if (x < this.lowest_x)
            {
                this.lowest_x = x;
            }
            if (x > this.highest_x)
            {
                this.highest_x = x;
            }
            if (y > this.highest_y)
            {
                this.highest_y = y;
            }
        }
        
    }
}
