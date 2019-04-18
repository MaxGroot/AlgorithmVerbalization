using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class SnapShot
    {
        private string location;
        private string model_extension;
        private string rules_extension;
        private string drawing_extension;
        private DotExport drawer;
        private TextWriter writer;

        public SnapShot(TextWriter writer, string location, string model_extension, string rules_extension, string drawing_extension)
        {
            this.writer = writer;
            this.location = location;
            this.model_extension = model_extension;
            this.rules_extension = rules_extension;
            this.drawing_extension = drawing_extension;

            drawer = new DotExport();
        }
        public void Make(string name, DecisionTree tree)
        {
            // Save model
            writer.set_location(location);
            writer.filesave_lines(name + "." + this.model_extension, ModelManager.generate_model(tree));

            // Save model as rule set
            writer.filesave_lines(name + "." + this.rules_extension, ModelManager.generate_rules(tree));

            // Save image
            writer.filesave_lines(name + "." + this.drawing_extension, drawer.lines(tree));

            // Lets see how well it classifies.
            List<DataInstance> total_set = new List<DataInstance>();
            foreach(Leaf leaf in tree.data_locations.Keys.ToList())
            {
                total_set.AddRange(tree.data_locations[leaf]);    
            }
            int correct_classifications = 0;
            foreach(DataInstance instance in total_set)
            {
                if (tree.classify(instance) == instance.getProperty(tree.target_attribute))
                {
                    correct_classifications++;
                }
            }
            double succesPercentage = Math.Round(((double) correct_classifications / (double) total_set.Count) * 100, 2);

            Console.WriteLine($"[SNAPSHOT] : Snapshot {name} made with success rate of {succesPercentage}% ({correct_classifications} / {total_set.Count}).");
        }
    }
}
