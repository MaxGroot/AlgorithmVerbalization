using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    interface Algorithm
    {
        DecisionTree train(List<DataInstance> examples, string target_attribute, Dictionary<string, string> attributes, Agent runner, Dictionary<string, object> parameters);
    }
}
