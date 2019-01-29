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
       private SystemState lastState = null;
       public Agent(ThoughtsManager thoughts)
        {
            this.thoughts = thoughts;
        }
       
       public abstract DecisionTree TRAIN(ObservationSet set);
       public abstract string ASK();

        public void THINK(SystemStateDescriptor descriptor, SystemState state)
        {
            this.thoughts.add_thought(descriptor.cause, descriptor.name, state);

            lastState = state;
        }

        public void DO(SystemStateDescriptor descriptor, string action, SystemState state = null)
        {
            // TODO.
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
