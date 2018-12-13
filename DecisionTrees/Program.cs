using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DecisionTrees
{
    class Program
    {
        static void Main(string[] args)
        {
            DataController import = new DataController();
            Console.WriteLine("Good Day!");
            Console.WriteLine("Program started. Enter the file path to import data from. ");
            string location = TextWriter.askLocation(1);
            ObservationSet observations =  import.importExamples(location);


            Console.WriteLine("Enter the directory to export data to. ");
            location = TextWriter.askLocation(2);

            Console.WriteLine("Catch error and output?");
            string catchinput = TextWriter.askLocation(3);
            bool catcherror = (catchinput == "TRUE");

            TextWriter writer = new TextWriter(location);

            Console.WriteLine("ADD UTILITY KNOWLEDGE");
            Agent agent = new ID3Agent(writer);

            Console.WriteLine("TELL");
            agent.TELL(observations); 
            
            Console.WriteLine("TOLD. Press a key to start training process \n");
            Console.ReadKey(true);

            // Train the algorithm based on the Training set
            Console.WriteLine("Starting Training process (TRAIN).");
            DecisionTree model = new DecisionTree();
            if (catcherror)
            {
                try
                {
                    model = agent.TRAIN();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Encountered an error! Writing output and model anyways.");
                    writer.write();
                    throw (e);
                }
            } else
            {
                model = agent.TRAIN();
            }
            Console.WriteLine("Training completed. Processing thoughts.");
            writer.write();

            Console.WriteLine("Thoughts processed. Processing model.");
            ModelManager.save(model, location);

            Console.WriteLine("Model saved. Saving image.");
            Drawing drawer = new Drawing(model);
            File.WriteAllLines(location + "drawing.txt", drawer.lines());
            Console.WriteLine("Image saved.");
            Console.ReadKey(true);
            
        }
    }
}
