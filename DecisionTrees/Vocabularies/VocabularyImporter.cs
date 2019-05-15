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
            
        }
    }
}
