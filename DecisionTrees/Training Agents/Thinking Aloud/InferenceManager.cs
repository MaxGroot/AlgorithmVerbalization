using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class InferenceManager
    {
        private Vocabulary vocabulary;
        public Dictionary<StateDescriptor, List<StateRecording>> state_record = new Dictionary<StateDescriptor, List<StateRecording>>();
        
        public InferenceManager(Vocabulary vocabulary)
        {
            this.vocabulary = vocabulary;
            foreach(StateDescriptor descriptor in this.vocabulary.state_descriptors)
            {
                state_record[descriptor] = new List<StateRecording>();
            }
        }

        public StateRecording Add_Inference(string inference_id)
        {
            StateRecording ret = null;
            foreach(InferenceType inference in vocabulary.inferences)
            {
                if (inference.id == inference_id)
                {
                    ret = new StateRecording(inference_id, inference.descriptor);

                    this.state_record[inference.descriptor].Add(ret);
                    break;
                }
            }
            if (ret == null)
            {
                throw new Exception($"Vocabulary did not contain {inference_id}!");
            }
            return ret;
        }

        public void write(string location)
        {
            foreach(StateDescriptor desc in state_record.Keys.ToList())
            {
                Console.WriteLine(desc.id);
                foreach(StateRecording rec in state_record[desc])
                {
                    Console.WriteLine(rec.action);
                }
            }
        }
    }
}
