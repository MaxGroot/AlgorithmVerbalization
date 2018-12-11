using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ObservationSet: Premise
    {
        public List<DataInstance> instances;
        public string target_attribute;
        public List<string> attributes;

        public ObservationSet (List<DataInstance> examples, string target_attribute, List<string> attributes)
        {
            this.instances = examples;
            this.target_attribute = target_attribute;
            this.attributes = attributes;
        }
    }
}
