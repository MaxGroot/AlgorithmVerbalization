using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class InferenceFromSet
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
    }
}
