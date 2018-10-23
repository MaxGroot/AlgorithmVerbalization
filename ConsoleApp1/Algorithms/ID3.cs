using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class ID3 : Algorithm
    {
        private List<DataInstance> examples;
        private string target_attribute;
        private List<string> attributes;

        public DecisionTree train(List<DataInstance> examples, string target_attribute, List<string> attributes)
        {
            this.examples = examples;
            this.target_attribute = target_attribute;
            this.attributes = attributes;

            return new DecisionTree();
        }

        public double entropy(List<DataInstance> S, string target_attribute)
        {
            Dictionary<string, float> proportions = new Dictionary<string, float> ();

            foreach(DataInstance example in S)
            {
                proportions[example.getProperty(target_attribute)]++;
            }

            double result = 0;
            foreach(var item in proportions)
            {
                double proportion = (item.Value / S.Count());
                result -= (proportion * Math.Log(proportion, 2));
            }

            return result;
        }

        public static float gain(List<DataInstance> S, string targetAttribute)
        {
            return 0f;
        }
    }
}
