﻿using System;
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
            string input_location = writer.askFromConfig("Enter the file path to import data from. ", "GENERAL", "input-location");
            string snapshot_location = writer.askFromConfig("Enter the directory to output snapshots to. ", "EXPORT", "snapshot-location");
            string thoughts_location = writer.askFromConfig("Enter the directory to output thoughts to. ", "EXPORT", "thoughts-location");
            
            string model_extension = "txt";
            string drawing_extension = "GRAPH";
            string thoughts_filename = "thoughts.csv";

            DataController import = new DataController();
            ObservationSet observations = import.importExamples(input_location);
            
            string catchinput = writer.askFromConfig("Catch error and output?", "GENERAL", "output-on-error");
            bool catcherror = (catchinput == "TRUE");

            ThoughtsManager thoughts = new ThoughtsManager();
            SnapShot snapShot = new SnapShot(writer, snapshot_location, model_extension, drawing_extension);
          
            Console.WriteLine("ADD UTILITY KNOWLEDGE");

            string algorithm = writer.askFromConfig("What algorithm should be used? [ID3, C4.5]", "GENERAL", "algorithm");
            Agent agent = null;
            switch (algorithm)
            {
                case "ID3":
                    agent = new ID3Agent(thoughts, snapShot);
                    break;
                case "C4.5":
                    agent = new C45Agent(thoughts, snapShot);
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
            if (catcherror)
            {
                try
                {
                    agent.TRAIN(observations);
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
                agent.TRAIN(observations);
            }
            long training_time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Training completed. Processing thoughts.");

            writer.set_location(thoughts_location);
            long thought_time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Training time: {training_time}ms. Thoughts processing time: {thought_time - training_time}ms.");
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
