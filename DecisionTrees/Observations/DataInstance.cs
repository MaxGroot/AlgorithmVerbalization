using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    public class DataInstance
    {
        Dictionary<string, string> fields = new Dictionary<string, string> ();
        public string identifier;
        public DataInstance(string identifier)
        {
            this.identifier = identifier;
        }
        // Set an attribute to a value of this Data Instance. 
        public DataInstance setProperty(string attribute, string value)
        {
            this.fields.Add(attribute, value);
            return this;
        }

        // Access an attribute of this Data Instance
        public string getProperty(string attribute)
        {
            if (this.fields.ContainsKey(attribute))
            {
                return this.fields[attribute];
            }
            throw new Exception($"Dictionary did not contain {attribute}");
        }
        public double getPropertyAsDouble(string attribute)
        {
            string my_value_as_a_string = getProperty(attribute);
            double my_value;
            bool conversion_succesful = double.TryParse(my_value_as_a_string, out my_value);
            if (!conversion_succesful)
            {
                throw new Exception($"Could not convert {my_value_as_a_string} to a double");
            }
            return my_value;
        }
        public DataInstance overwriteProperty(string attribute, string value)
        {
            this.fields[attribute] = value;
            return this;
        }
    }
}
