using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class Agent
    {
       private ThoughtsManager thoughts;
       private SnapShot snapShot;
       private SystemState lastState = null;
       private Algorithm algorithm;

        public Agent(Algorithm algorithm, ThoughtsManager thoughts, SnapShot snapShot)
        {
            this.algorithm = algorithm;
            this.thoughts = thoughts;
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
