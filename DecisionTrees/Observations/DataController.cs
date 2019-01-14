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
        
        public ObservationSet importExamples(string filepath)
        {
            return this.importCSV(filepath, true);
        }

        public ObservationSet importUnclassified(string filepath)
        {
            return this.importCSV(filepath, false);
        }

        public void exportSet(string filepath, ObservationSet export)
        {
            string writestring = exportCSV_string(export);
            File.WriteAllText(filepath, writestring);
        }

        private ObservationSet importCSV(string filepath, bool set_classifiers)
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
                int classifier_column = -1;

                if (row != 1)
                {
                    // This is an instance. The attributes have already been established.
                    DataInstance addition = new DataInstance();
                    int column = 0;

                    // Find the properties of this instance. 
                    foreach (string value in values)
                    {
                        // If we do not want to set the classifiers of the instances, we need to check we have not arrived at the 
                        // Final column.
                        if (  set_classifiers || column != classifier_column ) 
                        {
                            addition.setProperty(all_attributes[column], value);
                        }
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
                            classifier_column = column;
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

        private string exportCSV_string(ObservationSet export)
        {
            string seperator = ";";
            var csv = new StringBuilder();
            
            // Generate the first line
            string firstline = "";
            foreach(string attr_name in export.attributes)
            {
                firstline += attr_name;
                firstline += seperator;
            }
            firstline += export.target_attribute + seperator;
            csv.AppendLine(firstline);
            foreach (DataInstance instance in export.instances)
            {
                string my_line = "";
                foreach (string attr_name in export.attributes)
                {
                    my_line += instance.getProperty(attr_name);
                    my_line += seperator;
                }
                my_line += instance.getProperty(export.target_attribute);
                my_line += seperator;

                csv.AppendLine(my_line);
            }
            return csv.ToString();
        }
    }
}
