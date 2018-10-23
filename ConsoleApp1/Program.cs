using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program started");

            TrainingController trainer = new TrainingController();

            Algorithm algorithm = new ID3();

            // Load in everything we need for the training process
            List <DataInstance> exampleSet = trainer.exampleSet();
            List <string> attributeSet = trainer.exampleAttributes();
            string targetAttribute = trainer.targetAttribute();
            Console.WriteLine("Training data loaded. ");

            // Train the algorithm based on the Training set
            Console.WriteLine("Starting Training process.");

            algorithm.train(exampleSet, targetAttribute, attributeSet);

            Console.WriteLine("Training completed. Press any key to close.");
            Console.ReadKey(true);
        }
    }
}
