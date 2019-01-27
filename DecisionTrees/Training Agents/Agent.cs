using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    abstract class Agent
    {
       private ThoughtsManager thoughts;

       public Agent(ThoughtsManager thoughts)
        {
            this.thoughts = thoughts;
        }
       
       public abstract DecisionTree TRAIN(ObservationSet set);
       public abstract string ASK();

        public void THINK(string occurence, string action, SystemState state)
        {
            this.thoughts.add_thought(occurence, action, state);
        }

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
