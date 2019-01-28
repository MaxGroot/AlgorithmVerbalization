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
        
        public override string ASK()
        {
            throw new NotImplementedException();
        }

        public ID3Agent(ThoughtsManager thoughts): base(thoughts)
        {

        }

        public override DecisionTree TRAIN(ObservationSet set)
        {
            return algorithm.train(set.instances, set.target_attribute, set.attributes.Keys.ToList(), this);
        }
    }
}
