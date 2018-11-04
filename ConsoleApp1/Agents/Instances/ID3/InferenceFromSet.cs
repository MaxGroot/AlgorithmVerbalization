using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class InferenceFromSet : Inference
    {
       private ObservationSet observations;

       public void set(ObservationSet observations)
        {
            this.observations = observations;
        }
       
        public ObservationSet get()
        {
            return this.observations;
        }

        public bool tell(Premise premise)
        {
            ObservationSet input = (ObservationSet) premise;
            if (input == null)
            {
                return false;
            }
            this.observations = input;
            return true;
        }
    }
}
