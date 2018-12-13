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
            Console.WriteLine("Good Day!");

            // Ask the important questions.
            Console.WriteLine("Program started. Enter the file path to import data from. ");
            string location = TextWriter.askLocation(1);

            DataController import = new DataController();
            ObservationSet observations =  import.importExamples(location);


            Console.WriteLine("Enter the directory to export data to. ");
            location = TextWriter.askLocation(2);

            Console.WriteLine("Catch error and output?");
            string catchinput = TextWriter.askLocation(3);
            bool catcherror = (catchinput == "TRUE");


            // Prepare outputting
            TextWriter writer = new TextWriter(location);
            ThoughtsManager thoughts = new ThoughtsManager();
            string model_filename = "decisiontree.txt";
            string drawing_filename = "drawing.txt";
            string thoughts_filename = "thoughts.csv";


            Console.WriteLine("ADD UTILITY KNOWLEDGE");
            Agent agent = new ID3Agent(thoughts);

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
                    writer.filesave_string(thoughts_filename, thoughts.output());
                    throw (e);
                }
            } else
            {
                model = agent.TRAIN();
            }
            Console.WriteLine("Training completed. Processing thoughts.");
            writer.filesave_string(thoughts_filename, thoughts.output());

            Console.WriteLine("Thoughts processed. Processing model.");
            writer.filesave_lines(model_filename, ModelManager.output(model));

            Console.WriteLine("Model saved. Saving image.");

            DrawManager drawing = new DrawManager(model);
            writer.filesave_lines(drawing_filename, drawing.lines());
            Console.WriteLine("Image saved.");
            Console.ReadKey(true);
            
        }
    }
}
