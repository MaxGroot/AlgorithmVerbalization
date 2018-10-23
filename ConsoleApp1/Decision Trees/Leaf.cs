using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Leaf
    {
        public string value_splitter;
        public string classifier;
        private Node parent;

        public Leaf(string value_splitter, string classifier, Node parent)
        {
            this.value_splitter = value_splitter;
            this.classifier = classifier;
            this.parent = parent;

            Console.WriteLine($"Leaf created for {value_splitter} leading to {classifier}");
        }
    }
}
