using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class Vocabulary
    {
        public List<InferenceType> inferences = new List<InferenceType>();
        public List<InferenceOutput> inference_outputs = new List<InferenceOutput>();
        public List<StateDescriptor> state_descriptors = new List<StateDescriptor>();
    }
}
