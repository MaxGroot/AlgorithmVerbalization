using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

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
            writer.set_location(location);

            string catchinput = writer.askFromConfig("Catch error and output?", "GENERAL", "output-on-error");
            bool catcherror = (catchinput == "TRUE");


            ThoughtsManager thoughts = new ThoughtsManager();
            string model_filename = "decisiontree.txt";
            string drawing_filename = "drawing.GRAPH";
            string thoughts_filename = "thoughts.csv";


            Console.WriteLine("ADD UTILITY KNOWLEDGE");

            string algorithm = writer.askFromConfig("What algorithm should be used? [ID3, C4.5]", "GENERAL", "algorithm");
            Agent agent = null;
            switch (algorithm)
            {
                case "ID3":
                    agent = new ID3Agent(thoughts);
                    break;
                case "C4.5":
                    agent = new C45Agent(thoughts);
                    break;
                default:
                    throw new Exception($"Unknown algorithm given: {algorithm}");
            }
            
            Console.WriteLine("ADDED. Press a key to start training process \n");
            Console.ReadKey(true);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Train the algorithm based on the Training set
            Console.WriteLine("Starting Training process (TRAIN).");
            DecisionTree model = new DecisionTree();
            if (catcherror)
            {
                try
                {
                    model = agent.TRAIN(observations);
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
                model = agent.TRAIN(observations);
            }
            long training_time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Training completed. Processing thoughts.");
            // writer.filesave_string(thoughts_filename, thoughts.output());
            long thought_processing_time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Thoughts processed. Processing model.");
            writer.filesave_lines(model_filename, ModelManager.output(model));
            long saving_time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("Model saved. Saving image.");
            DotExport drawer = new DotExport();
            writer.filesave_lines(drawing_filename, drawer.lines(model));

            long drawing_time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Image saved.");

            Console.WriteLine($"Training time: {training_time}. Thought processing time: {thought_processing_time - training_time}");
            Console.WriteLine($"Model saving time: {saving_time - thought_processing_time}. Drawing saving time: {drawing_time - saving_time}.");
        }

        static void classify(TextWriter writer)
        {
            ModelLoader loader = new ModelLoader();

            // Ask the important questions.
            string model_location = writer.askFromConfig("Enter the file path to import the model from. ", "CLASSIFICATION", "model-location");
            DecisionTree model = loader.load_model(model_location);
            
            // Import the classification data.
            string data_location = writer.askFromConfig("Enter the file path to import the data from. ", "CLASSIFICATION", "input-location");

            DataController datacontroller = new DataController();
            ObservationSet observations = datacontroller.importUnclassified(data_location);
            
            // Get ready for classification.
            string export_location = writer.askFromConfig("Enter the file path to export data to. ", "CLASSIFICATION", "output-location");

            Console.WriteLine("READY. Press a key to start classification process \n");
            Console.ReadKey(true);

            List<DataInstance> classified_instances = new List<DataInstance>();
            string classifier_name = observations.target_attribute;
            foreach(DataInstance instance in observations.instances)
            {
                classified_instances.Add(model.classify(instance, classifier_name));
            }
            ObservationSet export_set = new ObservationSet(classified_instances, classifier_name, observations.attributes);

            Console.WriteLine("Classification succesful, saving now.");
            datacontroller.exportSet(export_location, export_set);

        }
    }
}
