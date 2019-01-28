using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ObservationSet
    {
        public List<DataInstance> instances;
        public string target_attribute;
        public Dictionary<String, String> attributes;

        public ObservationSet (List<DataInstance> examples, string target_attribute, Dictionary<string, string> attributes)
        {
            this.instances = examples;
            this.target_attribute = target_attribute;
            this.attributes = attributes;
        }
    }
}
