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
            ImportController import = new ImportController();

            Console.WriteLine("Program started. Enter the file path to import data from. \n");
            ObservationSet observations =  import.importExamples(Console.ReadLine());

            Console.WriteLine("ADD UTILITY KNOWLEDGE");
            Agent agent = new ID3Agent();

            Console.WriteLine("TELL");
            agent.TELL(observations); 
            
            Console.WriteLine("TOLD. Press a key to start training process \n");
            Console.ReadKey(true);

            // Train the algorithm based on the Training set
            Console.WriteLine("Starting Training process (INFER).");

            agent.INFER();

            Console.WriteLine("Training completed. Press any key to close.");
            Console.ReadKey(true);
        }
    }
}
