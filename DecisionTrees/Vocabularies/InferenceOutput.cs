using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class InferenceOutput
    {
        public string filename;
        public List<string> inference_ids;
        public List<InferenceType> inferences = new List<InferenceType>();
    }
}
