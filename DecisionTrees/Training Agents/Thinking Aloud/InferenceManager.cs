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
        public List<String> all_actions_in_order = new List<String>();

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
                    this.all_actions_in_order.Add(inference.action);

                    break;
                }
            }
            if (ret == null)
            {
                throw new Exception($"Vocabulary did not contain {inference_id}!");
            }
            return ret;
        }

        public void write(TextWriter writer)
        {
            foreach(StateDescriptor descriptor in this.vocabulary.state_descriptors)
            {
                writer.filesave_string(descriptor.id + ".csv", csv_individual_descriptor(descriptor));
            }

            writer.filesave_lines("all_actions.txt", all_actions_in_order);
        }

        private string csv_individual_descriptor(StateDescriptor descriptor)
        {
            string seperator = ";";
            var csv = new StringBuilder();
            string firstline = "action";
            foreach (string variableName in descriptor.considerations.Keys.ToList())
            {
                firstline += $"{seperator}{variableName}";
            }
            firstline += seperator;
            csv.AppendLine(firstline);

            foreach (StateRecording recording in this.state_record[descriptor])
            {
                csv.AppendLine(recording.toLine(seperator));
            }

            return csv.ToString();
        }
    }
}
