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
           
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "no").setProperty("humidity","high").setProperty("temperature","hot").setProperty("outlook", "sunny"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "no").setProperty("humidity", "high").setProperty("temperature", "hot").setProperty("outlook", "sunny"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes").setProperty("humidity", "high").setProperty("temperature", "hot").setProperty("outlook", "overcast"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes").setProperty("humidity", "high").setProperty("temperature", "mild").setProperty("outlook", "rain"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes").setProperty("humidity", "normal").setProperty("temperature", "cool").setProperty("outlook", "rain"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "no").setProperty("humidity", "normal").setProperty("temperature", "cool").setProperty("outlook", "rain"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "yes").setProperty("humidity", "normal").setProperty("temperature", "cool").setProperty("outlook", "overcast"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "no").setProperty("humidity", "high").setProperty("temperature", "mild").setProperty("outlook", "sunny"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes").setProperty("humidity", "normal").setProperty("temperature", "cool").setProperty("outlook", "sunny"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes").setProperty("humidity", "normal").setProperty("temperature", "mild").setProperty("outlook", "rain"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "yes").setProperty("humidity", "normal").setProperty("temperature", "mild").setProperty("outlook", "sunny"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "yes").setProperty("humidity", "high").setProperty("temperature", "mild").setProperty("outlook", "overcast"));
            this.instances.Add((new DataInstance()).setProperty("wind", "weak").setProperty("play", "yes").setProperty("humidity", "normal").setProperty("temperature", "hot").setProperty("outlook", "overcast"));
            this.instances.Add((new DataInstance()).setProperty("wind", "strong").setProperty("play", "no").setProperty("humidity", "high").setProperty("temperature", "mild").setProperty("outlook", "rain"));

            this.attributes.Add("wind");
            this.attributes.Add("humidity");
            this.attributes.Add("temperature");
            this.attributes.Add("outlook");

            this.target = "play";
        }
    }
}
