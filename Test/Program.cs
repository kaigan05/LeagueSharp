using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<Char> chars = "abc".ToList();
            List<string> allCombinations = new List<String>();
            for (int i = 1; i <= chars.Count; i++)
            {
                var combis = new Facet.Combinatorics.Combinations<Char>(
                    chars, i, Facet.Combinatorics.GenerateOption.WithoutRepetition);
                allCombinations.AddRange(combis.Select(c => string.Join("", c)));
            }

            foreach (var combi in allCombinations)
                Console.WriteLine(combi);
            Console.ReadLine();
        }
    }
}
