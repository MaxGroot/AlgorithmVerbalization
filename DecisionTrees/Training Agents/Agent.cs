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
        private SnapShot snapShot;
       private SystemState lastState = null;
       public Agent(ThoughtsManager thoughts, SnapShot snapShot)
        {
            this.snapShot = snapShot;
            this.thoughts = thoughts;
        }
       
       public abstract DecisionTree TRAIN(ObservationSet set);
       public abstract string ASK();

        public void SNAPSHOT(string name,DecisionTree tree)
        {
            this.snapShot.Make(name, tree);
        }

        public void THINK(EventDescriptor descriptor, params object[] state_params)
        {
            SystemState state = new SystemState(state_params).setDescriptor(descriptor);
            this.thoughts.add_thought(descriptor.cause, descriptor.name, state);
        }

        public void DO(EventDescriptor descriptor, string action, SystemState state = null)
        {
            // TODO.
        }

        public void prepare_system_state(List<EventDescriptor> descriptors)
        {
            foreach(EventDescriptor descriptor in descriptors)
            {
                this.thoughts.add_systemstate_descriptor(descriptor);
            }
            this.thoughts.generate_total_descriptor();
        }
    }
}
