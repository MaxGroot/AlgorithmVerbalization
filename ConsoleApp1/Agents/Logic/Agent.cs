using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    abstract class Agent
    {
       private List<InputInference> inferences = new List<InputInference>();
       private TextWriter writer;

       public Agent()
        {
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
            this.writer.infer_add(state);
        }
        
        public void DECIDE(Decision decision, int level)
        {
            for(int i=0; i<level; i++)
            {
                decision.appliedaction = "\t" + decision.appliedaction;
            }
            this.writer.decision_add(decision);
        }
        
        // TODO: REFACTOR IN CONSTRUCTOR
        public void addInference(InputInference inference)
        {
            this.inferences.Add(inference);
        }

        public abstract void addInferences();

        public void addWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void prepare_system_state(List<SystemStateDescriptor> descriptors)
        {
            foreach(SystemStateDescriptor descriptor in descriptors)
            {
                this.writer.add_systemstate_descriptor(descriptor);
            }
            this.writer.generate_total_descriptor();
        }
    }
}
