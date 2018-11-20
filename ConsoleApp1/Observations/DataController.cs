using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DecisionTrees
{
    class DataController
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

        public ObservationSet generateExamples()
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

            return new ObservationSet(this.exampleSet(), this.targetAttribute(), this.exampleAttributes());
        }
        public ObservationSet importExamples(string filepath)
        {
            var reader = new StreamReader(filepath);
            int row = 0;
            char sep = ';';
            List<string> all_attributes = new List<string>();

            // Loop through CSV lines. 
            while (!reader.EndOfStream)
            {
                row++;
                string line = reader.ReadLine();
                var values = line.Split(sep);
                if (row != 1)
                {
                    // This is a training instance. The attributes have already been established.
                    DataInstance addition = new DataInstance();
                    int column = 0;

                    // Find the properties of this training example.
                    foreach (string value in values)
                    {
                        addition.setProperty(all_attributes[column], value);
                        column++;
                    }
                    this.instances.Add(addition);
                }
                else
                {
                    int column = 0;
                    // This line describes the attributes.
                    foreach (string value in values)
                    {
                        all_attributes.Add(value);
                        if (column == values.Length - 1)
                        {
                            // This is the last column. It is the target attribute
                            this.target = value;
                        }
                        else
                        {
                            // Add it to the columns-that-are-not-the-classifier list.
                            this.attributes.Add(value);
                        }

                        column++;
                    }
                }
            }

            // All data has been considered, let's return a ObservationSet.
            return new ObservationSet(this.exampleSet(), this.targetAttribute(), this.exampleAttributes());
        }
    }
}
