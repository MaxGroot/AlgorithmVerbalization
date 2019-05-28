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

        char seperator;

        public InferenceManager(Vocabulary vocabulary, char csv_seperator)
        {            
            this.vocabulary = vocabulary;
            this.seperator = csv_seperator;

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

                    if (inference.descriptor != null)
                    {
                        this.state_record[inference.descriptor].Add(ret);
                    }
                    this.all_actions_in_order.Add(inference.action);

                    break;
                }
            }
            if (ret == null)
            {
                throw new Exception($"Vocabulary did not contain {inference_id}!");
            }

                Console.Write($"\r{all_actions_in_order.Count} inferences (last inference: {inference_id})         ");
            
            return ret;
        }

        public void write(TextWriter writer)
        {
            foreach(StateDescriptor descriptor in this.vocabulary.state_descriptors)
            {
                writer.filesave_string(descriptor.id + ".csv", csv_individual_descriptor(descriptor));
            }
            List<string> process_log = new List<string>();
            process_log.Add("case;event");
            string process_case = "Case 0";
            foreach(string action in all_actions_in_order)
            {
                process_log.Add($"{process_case};{action}");
            }
            writer.filesave_lines("process_log.csv", process_log);
        }

        private string csv_individual_descriptor(StateDescriptor descriptor)
        {
            var csv = new StringBuilder();
            string firstline = "";
            List<string> keys_in_order = descriptor.considerations.Keys.ToList();
            foreach (string variableName in keys_in_order)
            {
                firstline += $"{variableName}{seperator}";
            }
            firstline += "action" + seperator;
            csv.AppendLine(firstline);

            foreach (StateRecording recording in this.state_record[descriptor])
            {
                csv.AppendLine(recording.toLine(seperator, keys_in_order));
            }

            return csv.ToString();
        }
    }
}
