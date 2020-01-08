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

            var projectPath = "C:\\Users\\Tomas\\Desktop\\Projektinis\\LibAdapter";
            var mainDllPath = "C:/Users/Tomas/Desktop/Projektinis/LibAdapter/bin/Debug/netcoreapp2.2/LibAdapter.dll";

            var references = Assembly.LoadFile(mainDllPath).GetReferencedAssemblies();

            var compilation = CSharpCompilation.Create("Code")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(mainDllPath));

            foreach (var name in references)
            {
                Assembly assembly;

                try
                {
                    assembly = Assembly.Load(name);
                }
                catch
                {
                    assembly = Assembly.LoadFile($"C:\\Users\\Tomas\\Desktop\\Projektinis\\LibAdapter\\bin\\Debug\\netcoreapp2.2\\{name.Name}.dll");
                }

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

            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var tree in trees)
            {
                var map = new SyntaxTypeMap(tree);
                map.PopulateFromCompilation(compilation);
                maps.Add(map);
            }
            watch.Stop();
            Console.WriteLine("Loading took:" + watch.ElapsedMilliseconds);

            #endregion
            
            watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var action in actions)
            {
                foreach (var map in maps)
                {
                    map.Root = (CompilationUnitSyntax) action.ToVisitor(map).Visit(map.Root);
                }
            }
            watch.Stop();
            Console.WriteLine("Refactoring took:" + watch.ElapsedMilliseconds);

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
