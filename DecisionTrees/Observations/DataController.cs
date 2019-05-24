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

        char seperator, dec;

        public DataController(char seperator, char dec)
        {
            this.seperator = seperator;
            this.dec = dec;
        }

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
            int classifier_column = -1;
            // Loop through CSV lines. 
            while (!reader.EndOfStream)
            {
                row++;
                string line = reader.ReadLine();
                var values = line.Split(this.seperator);

                if (row > 2)
                {
                    // This is an instance. The attributes have already been established.
                    DataInstance addition = new DataInstance(ElementHelper.generateElementId('D', instances.Count));
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
                            if (value == "")
                            {
                                addition.setProperty(key, null);
                            } else
                            {
                                string value_to_set = value;
                                if (key != this.target && this.attributes[key] == "continuous")
                                {
                                    value_to_set = value.Replace(',', this.dec);
                                    value_to_set = value.Replace('.', this.dec);
                                }
                                addition.setProperty(key, value_to_set);
                            }
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
                            // Add it to the columns list.
                            this.column_positions.Add(value);
                            this.attributes.Add(value, "UNKNOWN");
                            column++;
                        }
                    }else if( row == 2)
                    {
                        int column = 0;
                        // This line describes the attribute types
                        foreach(string value in values)
                        {
                            if (value == "classifier")
                            {
                                // Classifier column.
                                // First we remove it from the attributes dictionary, since we do not want the classifier to show up in the attributes.
                                this.attributes.Remove(this.column_positions[column]);

                                // Then, we make sure that we know the correct classifier.
                                this.target = this.column_positions[column];
                                classifier_column = column;
                            }else
                            {
                                if (value != "nominal" && value != "continuous")
                                {
                                    throw new Exception($"Unknown attribute type {value}.");
                                }
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
