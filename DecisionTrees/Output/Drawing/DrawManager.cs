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
            List<DrawElement> all_elements = new List<DrawElement>();

            List<DrawElement> queue = new List<DrawElement>();

            // Create the root
            DrawElement root_element = new DrawElement(tree.getRoot(), "" , tree.getRoot().label, 0, 0);
            all_elements.Add(root_element);

            // Add the root's kids to the queue.
            queue.Add(root_element);


            // Iterate!
            while (queue.Count > 0)
            {
                iterate(ref queue, ref all_elements);
            }
            return generate_image_from_elements(all_elements);
        }

        private void iterate(ref List<DrawElement> queue, ref List<DrawElement> all_elements)
        {
            // Manage queue
            Node node = queue[0].node;
            int currentX = queue[0].x;
            int currentY = queue[0].y;
            
            queue.RemoveAt(0);

            // LETS MAKE BABIES. 
            // First, find out some global stuff
            int node_children_count = node.getNodeChildren().Count;
            int leaf_children_count = node.getLeafChildren().Count;
            int children_count = node_children_count + leaf_children_count;

            // child_offset determines how much space offset to this node's position the children should start / finish. 
            // It depends on the amount of children it will have. 4 --> 2 , 5 --> 2
            int child_offset = (int)Math.Floor( (double) (children_count / 2));

            // Added width is width taken up by the labels and value_splitters of this node and its leafchildren. 
            int child_width = 0;

            List<DrawElement> node_babies = new List<DrawElement>();
            int i = 0;
            foreach(ITreeElement baby in node.getAllChildren())
            {
                // Determine coordinates
                int x = currentX;
                x += (i - child_offset);
                int y = currentY + 2;

                DrawElement el = null;
                if (baby is Node)
                {
                    el = new DrawElement((Node) baby, baby.line(), baby.underline(), x, y);
                    node_babies.Add(el);
                }
                else
                {
                    el = new DrawElement(null, baby.line(), baby.underline(), x, y);
                }

                move_other_elements(ref all_elements, x, added_width(oddsize(baby.line()), oddsize(baby.underline())));

                all_elements.Add(el);

                outputImage(generate_image_from_elements(all_elements));
                i+=2;
            }
            queue.AddRange(node_babies);
        }

        // Move other elements aside with a certain amount according to if they are left or right of the given threshold.
        private void move_other_elements(ref List<DrawElement> all_elements, int x_threshold, int movement)
        {
            Console.WriteLine($"MOVE {x_threshold} : {movement}");
            foreach(DrawElement el in all_elements)
            {
                if (el.x < x_threshold)
                {
                    el.x -= movement;
                    update_coordinate_variables(el.x, el.y);
                }
                if (el.x > x_threshold)
                {
                    el.x += movement;
                    update_coordinate_variables(el.x, el.y);
                }
                if (el.x == x_threshold)
                {
                    el.x += movement;
                    update_coordinate_variables(el.x, el.y);
                }
            }
            foreach(DrawElement el in all_elements)
            {
                Console.WriteLine($"[{el.line}-{el.underline}]: ({el.x},{el.y})");
            }
        }

        // Make sure the lowest_x, highest_x and highest_y variables are updated when needed.
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
        
        // Add a space before the input string to ensure that all strings that leave this function have an odd length. 
        private string oddsize(string input)
        {
            if (input == null)
            {
                return " ";
            }
            if (input.Length % 2 == 0)
            {
                return " " + input;
            }
            return input;
        }

        // Returns the highest added width of a collection of strings. 
        private int added_width(string a, string b)
        {
            int[] values = {    (int)Math.Floor((double)(a.Length / 2)),
                                (int)Math.Floor((double)(b.Length / 2))
                            };

            return values.Max();
            
        }

        // Given a line and an underline, insert the characters corresponding to the element into those lines at the correct positions.
        private string[] insert_element_into_lines(string line, string underline, DrawElement el)
        {
            StringBuilder lineB = new StringBuilder(line);
            StringBuilder underlineB = new StringBuilder(underline);

            int offset_line = (int)Math.Floor((double)(el.line.Length / 2));
            int offset_underline = (int)Math.Floor((double)el.underline.Length / 2);

            for(int i = 0; i<el.line.Length; i++)
            {
                lineB[el.x - offset_line + i] = el.line[i];
            }
            for(int i= 0; i<el.underline.Length; i++)
            {
                underlineB[el.x - offset_underline + i] = el.underline[i];
            }

            return new string[] { lineB.ToString(), underlineB.ToString()};
        }

        private List<string> generate_image_from_elements(List<DrawElement> input_elements)
        {
            List<string> lines = new List<string>();
            List<DrawElement> all_elements = new List<DrawElement>();

            // Running this code will affect the following instance variables. We will roll them back after the image generation is done. 
            int old_low_x = lowest_x;
            int old_high_x = highest_x;
            int old_high_y = highest_y;

            foreach (DrawElement el in input_elements)
            {
                all_elements.Add(DrawElement.Copy(el));
            }

            // We don't want negative X values so we move all elements to the right with the difference to 0 of the most leftwards element.
            foreach (DrawElement el in all_elements)
            {
                el.x += Math.Abs(lowest_x);
            }
            // All elements have at least x 0 but that might not have enough spacing for the labels they have.
            foreach (DrawElement el in all_elements)
            {
                int my_offside = added_width(oddsize(el.line), oddsize(el.underline));
                while (el.x - my_offside < 0)
                {
                    foreach(DrawElement inner_element in all_elements)
                    {
                        inner_element.x += 1;
                        update_coordinate_variables(inner_element.x, inner_element.y);
                    }
                }
            }

            // Adjust the variable of highest X. 
            highest_x += Math.Abs(lowest_x);

            // Adjust for the width of the longest length label?
            foreach(DrawElement el in all_elements)
            {
                int my_offside = added_width(oddsize(el.line), oddsize(el.underline));
                if (el.x + my_offside > highest_x)
                {
                    highest_x = el.x + my_offside;
                }
            }

            // Adjust for the labels on the left?


            // Each element now knows it's own position. They are placed in a raster that starts at 0,0. 
            // We loop through them and write it in lines.
            for (int j = 0; j <= highest_y + 10; j += 1)
            {
                string l = "";
                for(int z= 0; z<= highest_x; z++)
                {
                    l += " ";
                }
                lines.Add(l);
            }
            foreach (DrawElement el in all_elements)
            {
                string[] updated_lines = insert_element_into_lines(lines[el.y], lines[el.y + 1], el);
                lines[el.y] = updated_lines[0];
                lines[el.y + 1] = updated_lines[1];
            }
            lines.Insert(0, new string('-', highest_x + 1));
            lines.Add(new string('-', highest_x + 1));
            // Rollback instance variables.
            this.highest_x = old_high_x;
            this.lowest_x = old_low_x;
            this.highest_y = old_high_y;

            return lines;
        }

        private void outputImage(List<string> lines)
        {
            foreach(string l in lines)
            {
                Console.WriteLine(l);
            }
        }
    }
}
