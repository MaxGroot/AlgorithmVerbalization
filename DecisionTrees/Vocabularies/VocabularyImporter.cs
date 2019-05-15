using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace DecisionTrees
{
    class VocabularyImporter
    {
        public Vocabulary vocabulary;
        public void import(string location)
        {
            string file_contents = File.ReadAllText(location);
            this.vocabulary = JsonConvert.DeserializeObject<Vocabulary>(file_contents);
            
            foreach(InferenceType inferencetype in vocabulary.inferences)
            {
                StateDescriptor descriptor = vocabulary.state_descriptors.Find(d => d.id == inferencetype.state_descriptor_id);
                if (descriptor == null)
                {
                    throw new Exception($"Inference {inferencetype.id} requires state descriptor {inferencetype.state_descriptor_id} but no such descriptor was found.");
                }
                inferencetype.descriptor = descriptor;
            } 
        }
    }
}
