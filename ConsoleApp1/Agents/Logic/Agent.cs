using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    interface Agent
    {
        void TELL(Premise premise);

        DecisionTree INFER();

        string ASK();
    }
}
