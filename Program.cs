using System;
using System.Linq;

namespace LibAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            var actions = ChangeFileParser
                .ParseFile("C:\\Users\\Tomas\\Desktop\\Projektinis\\LibAdapter\\Changes.txt")
                .ToList();

            var syntaxTypeMap = SyntaxTypeMap.FromProject(
                "C:\\Users\\Tomas\\Desktop\\Projektinis\\LibAdapterTestSolution",
                "C:/Users/Tomas/Desktop/Projektinis/LibAdapterTestSolution/bin/Debug/netcoreapp3.0/LibAdapterTestSolution.dll");

            foreach (var tree in syntaxTypeMap.Trees)
            {
                var root = tree.GetRoot();
                foreach (var action in actions)
                {
                    root = action.ToVisitor(syntaxTypeMap).Visit(root);
                }

                Console.WriteLine(root);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
