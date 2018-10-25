using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class Program
    {
        static void Main(string[] args)
        {
            TrainingController trainer = new TrainingController();

            Algorithm algorithm = new ID3();

            Console.WriteLine("Program started. Enter the file path to import data from. \n");

            trainer.importExamples(Console.ReadLine());
            
            // Load in everything we need for the training process
            List <DataInstance> exampleSet = trainer.exampleSet();
            List <string> attributeSet = trainer.exampleAttributes();
            string targetAttribute = trainer.targetAttribute();

            Console.WriteLine("Training data loaded. Press a key to start training process \n");
            Console.ReadKey(true);

            // Train the algorithm based on the Training set
            Console.WriteLine("Starting Training process.");

            algorithm.train(exampleSet, targetAttribute, attributeSet);

            Console.WriteLine("Training completed. Press any key to close.");
            Console.ReadKey(true);
        }
    }
}
