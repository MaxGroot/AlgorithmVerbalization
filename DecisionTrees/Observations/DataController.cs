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
        
        private Dictionary<String , String> attributes = new Dictionary<String, String>();
        private List<String> column_positions = new List<String>();

        private string target = "";

        public List<DataInstance> exampleSet()
        {
            return this.instances;
        }

        public List<string> exampleAttributeNames()
        {
            return attributes.Keys.ToList();
        }
        public Dictionary<String, String> exampleAttributeTypes()
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
            int classifier_column = -1;
            // Loop through CSV lines. 
            while (!reader.EndOfStream)
            {
                row++;
                string line = reader.ReadLine();
                var values = line.Split(sep);

                if (row > 2)
                {
                    // This is an instance. The attributes have already been established.
                    DataInstance addition = new DataInstance(Calculator.generateElementId(instances.Count));
                    int column = 0;

                    // Find the properties of this instance. 
                    foreach (string value in values)
                    {
                        // If we do not want to set the classifiers of the instances, we need to check we have not arrived at the 
                        // Final column.
                        if (  set_classifiers || column != classifier_column ) 
                        {
                            string key = "ERR";
                            if (column == classifier_column)
                            {
                                 key = this.targetAttribute();
                            }else
                            {
                                 key = column_positions[column];
                            }
                            addition.setProperty(key, value);
                        }
                        column++;
                    }
                    this.instances.Add(addition);
                }
                else
                {
                    if (row == 1)
                    {
                        int column = 0;
                        // This line describes the attribute names.
                        foreach (string value in values)
                        {
                            if (column == values.Length - 1)
                            {
                                // This is the last column. It is the target attribute
                                this.target = value;
                                classifier_column = column;
                            }
                            else
                            {
                                // Add it to the columns-that-are-not-the-classifier list.
                                this.column_positions.Add(value);
                                this.attributes.Add(value, "UNKNOWN");
                            }
                            column++;
                        }
                    }else if( row == 2)
                    {
                        int column = 0;
                        // This line describes the attribute types
                        foreach(string value in values)
                        {
                            if (column == values.Length - 1)
                            {
                                // Last column.
                            }else
                            {
                                this.attributes[this.column_positions[column]] = value; 
                            }

                            column++;
                        }
                    }
                }
            }

            // All data has been considered, let's return a ObservationSet.
            return new ObservationSet(this.exampleSet(), this.targetAttribute(), this.exampleAttributeTypes());
        }

        private string exportCSV_string(ObservationSet export)
        {
            string seperator = ";";
            var csv = new StringBuilder();
            
            // Generate the first line
            string firstline = "";
            foreach(string attr_name in export.attributes.Keys.ToList())
            {
                firstline += attr_name;
                firstline += seperator;
            }
            firstline += export.target_attribute + seperator;
            csv.AppendLine(firstline);
            foreach (DataInstance instance in export.instances)
            {
                string my_line = "";
                foreach (string attr_name in export.attributes.Keys.ToList())
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
