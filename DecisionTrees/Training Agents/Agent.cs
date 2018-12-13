using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    abstract class Agent
    {
       protected List<InputInference> inferences = new List<InputInference>();
       private ThoughtsManager thoughts;

       public Agent(ThoughtsManager thoughts)
        {
            this.thoughts = thoughts;
            this.addInferences();
        }

       public bool TELL(Premise premise)
        {
            bool accepted = false;
            // Loop through inferences to check if they accept the premise.
            foreach(InputInference inference in this.inferences)
            {
                accepted = (inference.tell(premise) || accepted);
            }

            return accepted;
        }
       
       public abstract DecisionTree TRAIN();

       public abstract string ASK();

        public void INFER(SystemState state)
        {
            this.thoughts.infer_add(state);
        }
        
        public void DECIDE(Decision decision, int level)
        {
            for(int i=0; i<level; i++)
            {
                decision.appliedaction = "\t" + decision.appliedaction;
            }
            this.thoughts.decision_add(decision);
        }

        public abstract void addInferences();
        
        public void prepare_system_state(List<SystemStateDescriptor> descriptors)
        {
            foreach(SystemStateDescriptor descriptor in descriptors)
            {
                this.thoughts.add_systemstate_descriptor(descriptor);
            }
            this.thoughts.generate_total_descriptor();
        }
    }
}
