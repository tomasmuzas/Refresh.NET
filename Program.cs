using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LibAdapter.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Preparation
            var actions = ChangeFileParser
                .ParseFile("C:\\Users\\Tomas\\Desktop\\Projektinis\\LibAdapter\\Changes.txt")
                .ToList();

            var projectPath = "C:\\Users\\Tomas\\Desktop\\Projektinis\\LibAdapterTestSolution";
            var mainDllPath = "C:/Users/Tomas/Desktop/Projektinis/LibAdapterTestSolution/bin/Debug/netcoreapp3.0/LibAdapterTestSolution.dll";

            var references = Assembly.LoadFile(mainDllPath).GetReferencedAssemblies();

            var compilation = CSharpCompilation.Create("Code")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(mainDllPath));

            foreach (var name in references)
            {
                var assembly = Assembly.Load(name);
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(assembly.Location));
            }

            var trees = new List<SyntaxTree>();
            foreach (var filePath in Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories))
            {
                var code = File.ReadAllText(filePath);
                var tree = CSharpSyntaxTree.ParseText(code);
                tree = tree.WithRootAndOptions(new AnnotationVisitor().Visit(tree.GetRoot()), tree.Options);
                trees.Add(tree);
            }

            compilation = compilation.AddSyntaxTrees(trees);
            var maps = new List<SyntaxTypeMap>();
            foreach (var tree in trees)
            {
                var map = new SyntaxTypeMap(tree);
                map.PopulateFromCompilation(compilation);
                maps.Add(map);
            }
            #endregion
            
            foreach (var action in actions)
            {
                foreach (var map in maps)
                {
                    map.Root = (CompilationUnitSyntax) action.ToVisitor(map).Visit(map.Root);
                }
            }

            foreach (var map in maps)
            {
                Console.WriteLine(map.Root);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
