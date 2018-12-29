﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class DrawElement
    {
        public int x, y;
        public Node node;
        public string line;
        public string underline;
        public DrawElement(Node node, string line, string underline, int x, int y)
        {
            this.node = node;
            this.x = x;
            this.y = y;
            this.line = line;
            this.underline = underline;
        }
        
    }
}