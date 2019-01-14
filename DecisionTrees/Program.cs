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
            Console.WriteLine("Good Day! Program has started.");

            // Create our file handler
            TextWriter writer = new TextWriter();
            string mode = writer.askFromConfig("What mode do you want to do?", "GENERAL","mode");
            
            switch(mode)
            {
                case "training":
                    train(writer);
                    break;
                case "classification":
                    classify(writer);
                    break;
                default:
                    throw new Exception($"Unknown mode entered: {mode}");
            }
            Console.WriteLine("Program finished.");
            Console.ReadKey(true);

        }
        static void train(TextWriter writer)
        {
            // Ask the important questions.
            string location = writer.askFromConfig("Enter the file path to import data from. ", "GENERAL", "input-location");

            DataController import = new DataController();
            ObservationSet observations = import.importExamples(location);

            location = writer.askFromConfig("Enter the directory to export data to. ", "GENERAL", "export-location");

            string catchinput = writer.askFromConfig("Catch error and output?", "GENERAL", "output-on-error");
            bool catcherror = (catchinput == "TRUE");


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
            }
            else
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
        }

        static void classify(TextWriter writer)
        {
            Console.WriteLine("Classified.");
        }
    }
}
