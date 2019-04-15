using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class C45Agent : Agent
    {
        private Algorithm algorithm = new C45Algorithm();
        public C45Agent(ThoughtsManager thoughts, SnapShot snapShot) : base(thoughts, snapShot)
        {
        }

        public override string ASK()
        {
            throw new NotImplementedException();
        }

        public override DecisionTree TRAIN(ObservationSet set)
        {
            DecisionTree tree = this.algorithm.train(set.instances, set.target_attribute, set.attributes, this);
            this.SNAPSHOT("final", tree);
            return tree;
        }
    }
}
