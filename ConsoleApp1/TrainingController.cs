using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class TrainingController
    {

        private List<DataInstance> instances;
        private List<string> attributes;
        private string target;

        public List<DataInstance> exampleSet()
        {
            return this.instances;
        }

        public List<string> exampleAttributes()
        {
            return this.attributes;
        }

        public string targetAttribute()
        {
            return this.target;
        }
    }
}
