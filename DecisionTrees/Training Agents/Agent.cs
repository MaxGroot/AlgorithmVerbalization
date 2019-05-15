using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class Agent
    {
       private InferenceManager inferences;
       private SnapShot snapShot;
       private SystemState lastState = null;
       private Algorithm algorithm;

        public Agent(Algorithm algorithm, InferenceManager inferences, SnapShot snapShot)
        {
            this.algorithm = algorithm;
            this.inferences = inferences;
            this.snapShot = snapShot;
        }

        public DecisionTree TRAIN(ObservationSet set)
        {
            DecisionTree tree = this.algorithm.train(set.instances, set.target_attribute, set.attributes, this);
            this.SNAPSHOT("final", tree);
            return tree;
        }

        public void SNAPSHOT(string name,DecisionTree tree)
        {
            this.snapShot.Make(name, tree);
        }
  
        public StateRecording THINK(string inference_id)
        {
            return this.inferences.Add_Inference(inference_id);
        }
    }
}
