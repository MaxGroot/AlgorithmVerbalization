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
       private Algorithm algorithm;

        public Agent(Algorithm algorithm, InferenceManager inferences, SnapShot snapShot)
        {
            this.algorithm = algorithm;
            this.inferences = inferences;
            this.snapShot = snapShot;
        }

        public DecisionTree TRAIN(ObservationSet set, Dictionary<string, object> parameters)
        {
            string parameter_string = "TRAIN(";
            foreach(string key in parameters.Keys.ToList())
            {
                parameter_string += $"{key}={parameters[key]}";
                if (key != parameters.Keys.Last())
                {
                    parameter_string += ",";
                }
            }
            parameter_string += ")";
            Console.WriteLine(parameter_string);

            DecisionTree tree = this.algorithm.train(set.instances, set.target_attribute, set.attributes, this, parameters);
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
