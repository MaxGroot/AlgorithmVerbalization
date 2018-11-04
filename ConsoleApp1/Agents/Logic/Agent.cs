using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    abstract class Agent
    {
       private List<Inference> inferences = new List<Inference>();

        public Agent()
        {
            this.addInferences();
        }

       public bool TELL(Premise premise)
        {
            bool accepted = false;
            // Loop through inferences to check if they accept the premise.
            foreach(Inference inference in this.inferences)
            {
                accepted = (inference.tell(premise) || accepted);
            }

            return accepted;
        }

        public void addInference(Inference inference)
        {
            this.inferences.Add(inference);
        }

       public abstract DecisionTree TRAIN();

       public abstract string ASK();

       public abstract void addInferences();
    }
}
