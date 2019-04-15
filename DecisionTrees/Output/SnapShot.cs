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
        private string drawing_extension;
        private DotExport drawer;
        private TextWriter writer;

        public SnapShot(TextWriter writer, string location, string model_extension, string drawing_extension)
        {
            this.writer = writer;
            this.location = location;
            this.model_extension = model_extension;
            this.drawing_extension = drawing_extension;
            drawer = new DotExport();
        }
        public void Make(string name, DecisionTree tree)
        {
            // Save model
            writer.set_location(location);
            writer.filesave_lines(name + "." + this.model_extension, ModelManager.output(tree));

            // Save image
            writer.filesave_lines(name + "." + this.drawing_extension, drawer.lines(tree));

            Console.WriteLine($"[SNAPSHOT] : Snapshot {name} made.");
        }
    }
}
