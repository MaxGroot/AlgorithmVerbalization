using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTrees
{
    class ElementHelper
    {
        private static List<string> alphabet()
        {
            return new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }


        public static string generateElementId(char character, int counter)
        {
            int second_letter_count = counter % 26;
            int first_letter_count = (int)counter / 26;

            return character.ToString() + "-" + alphabet()[first_letter_count] + alphabet()[second_letter_count];
        }
    }
}
