using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DrawElement
    {
        public char character;
        public int x, y;
        public Node node;

        public DrawElement(Node node, char character, int x, int y)
        {
            this.character = character;
            this.node = node;
            this.x = x;
            this.y = y;
        }
    }
}
