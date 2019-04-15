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

        public ID3Agent(ThoughtsManager thoughts, SnapShot snapShot): base(thoughts, snapShot)
        {

        }

        public override DecisionTree TRAIN(ObservationSet set)
        {
            DecisionTree tree = algorithm.train(set.instances, set.target_attribute, set.attributes, this);
            this.SNAPSHOT("final", tree);
            return tree;
        }
    }
}
