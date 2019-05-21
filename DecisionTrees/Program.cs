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
            string input_location = writer.askFromConfig("Enter the file path to import data from. ", "GENERAL", "input-location");
            string snapshot_location = writer.askFromConfig("Enter the directory to output snapshots to. ", "EXPORT", "snapshot-location");
            string thoughts_location = writer.askFromConfig("Enter the directory to output inferences to. ", "EXPORT", "inference-location");
            string vocabulary_location = writer.askFromConfig("Enter the file path to import the vocabulary form ", "VOCABULARY", "location");
            string catchinput = writer.askFromConfig("Catch error and output?", "GENERAL", "output-on-error");
            bool catcherror = (catchinput == "TRUE");

            string model_extension = "txt";
            string rules_extension = "rules.txt";
            string drawing_extension = "GRAPH";

            DataController import = new DataController();
            ObservationSet observations = import.importExamples(input_location);

            VocabularyImporter vocab = new VocabularyImporter();
            vocab.import(vocabulary_location);

            InferenceManager inferences = new InferenceManager(vocab.vocabulary);

            Stopwatch stopwatch = new Stopwatch();
            SnapShot snapShot = new SnapShot(writer, stopwatch, snapshot_location, model_extension, rules_extension, drawing_extension);
          

            string algorithmChoice = writer.askFromConfig("What algorithm should be used? [ID3, C4.5]", "GENERAL", "algorithm");
            Algorithm algorithm = null;
            switch (algorithmChoice)
            {
                case "ID3":
                    algorithm = new ID3Algorithm();
                    break;
                case "C4.5":
                    algorithm = new C45Algorithm();
                    break;
                default:
                    throw new Exception($"Unknown algorithm given: {algorithm}");
            }
            Agent agent = new Agent(algorithm, inferences, snapShot);
            Console.WriteLine("READY. Press a key to start training process \n");
            Console.ReadKey(true);
            
            // Train the algorithm based on the Training set
            Console.WriteLine("Starting Training process (TRAIN).");
            Console.WriteLine("");
            stopwatch.Start();
            if (catcherror)
            {
                try
                {
                    agent.TRAIN(observations);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Encountered an error! Writing output and model anyways.");
                    writer.set_location(thoughts_location);
                    inferences.write(writer);
                    throw (e);
                }
            }
            else
            {
                agent.TRAIN(observations);
            }

            Console.WriteLine("");
            long training_time = stopwatch.ElapsedMilliseconds;
            long snapshot_time = training_time - snapShot.secondsBySnapShot;

            Console.WriteLine("Training completed. Processing thoughts.");
            
            writer.set_location(thoughts_location);
            inferences.write(writer);

            long thought_time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Training time: {training_time}ms including snapshotting, {snapshot_time}ms excluding. Thoughts processing time: {thought_time - training_time}ms.");
        }

        static void classify(TextWriter writer)
        {
            ModelLoader loader = new ModelLoader();
            DataController datacontroller = new DataController();


            // Check if the data is already classified, and thus we are in verification mode and will check how our model holds up.
            string verification_mode_string = writer.askFromConfig("Verification mode? (TRUE or FALSE) ", "CLASSIFICATION", "verification-mode");
            bool verification_mode = (verification_mode_string == "TRUE");


            // Import the classification data.
            string data_location = writer.askFromConfig("Enter the file path to import the data from. ", "CLASSIFICATION", "input-location");
            ObservationSet observations = verification_mode ? datacontroller.importExamples(data_location) : datacontroller.importUnclassified(data_location);
            string classifier_name = observations.target_attribute;

            // Import the model
            string model_location = writer.askFromConfig("Enter the file path to import the model from. ", "CLASSIFICATION", "model-location");
            DecisionTree model = loader.load_model(model_location, classifier_name);
            
            // Get ready for classification.
            string export_location = writer.askFromConfig("Enter the file path to export data to. ", "CLASSIFICATION", "output-location");

            Console.WriteLine($"READY for classification{(verification_mode ? " in verification mode" : "")}. Press a key to start classification process \n");
            Console.ReadKey(true);
            
            List<DataInstance> classified_instances = new List<DataInstance>();
            int correct_classifications = 0;
            foreach(DataInstance instance in observations.instances)
            {
                string prediction = model.classify(instance);
                if (verification_mode)
                {
                    if (instance.getProperty(classifier_name) == prediction)
                    {
                        correct_classifications++;
                    }
                }
                else
                {
                    instance.setProperty(classifier_name, model.classify(instance));
                }
                classified_instances.Add(instance);
            }
            ObservationSet export_set = new ObservationSet(classified_instances, classifier_name, observations.attributes);
            Console.WriteLine("Classification succesful, saving now.");

            if (verification_mode)
            {
                double succesPercentage = Math.Round(((double)correct_classifications / (double)classified_instances.Count) * 100, 2);
                Console.WriteLine($"Verification mode results: success rate of {succesPercentage}% ({correct_classifications} / {classified_instances.Count}).");
            }
            datacontroller.exportSet(export_location, export_set);

        }
    }
}
