using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    interface Output
    {
        string toLine(String seperator, SystemStateDescriptor total);
    }
}
