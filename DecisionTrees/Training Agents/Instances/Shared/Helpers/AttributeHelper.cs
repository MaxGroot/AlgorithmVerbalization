using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class AttributeHelper
    {
        public static Dictionary<string, string> CopyAttributeDictionary(Dictionary<string, string> input)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach(string key in input.Keys.ToList())
            {
                output.Add(key, input[key]);
            }

            return output;
        }
    }
}
