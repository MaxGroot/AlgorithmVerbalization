using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ID3Agent : Agent
    {
        private Algorithm algorithm = new ID3();
        private InferenceFromSet infer1 = new InferenceFromSet();

        public ID3Agent()
        {

        }

        public string ASK()
        {
            throw new NotImplementedException();
        }

        public DecisionTree INFER()
        {
            ObservationSet set = this.infer1.get();
            return algorithm.train(set.instances, set.target_attribute, set.attributes);
        }

        public void TELL(Premise premise)
        {
            infer1.set((ObservationSet) premise);   
        }
    }
}
