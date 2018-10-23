using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class TrainingController
    {

        private List<DataInstance> instances = new List<DataInstance> ();
        private List<string> attributes = new List<string> ();
        private string target = "";

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

        public void generateExamples()
        {
           
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "no"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "no"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "no"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "no"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "no"));

            this.attributes.Add("wind");

            this.target = "play";
        }
    }
}
