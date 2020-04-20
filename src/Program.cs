using System; 
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LibAdapter.Migrations;
using LibAdapter.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter 
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectPath = args[1];
            var mainDllPath = args[2];
            var migrationPath = args[3];

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
                    assembly = Assembly.LoadFile(args[3]);
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

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var migration = MigrationLoader.FromPath(migrationPath);

            foreach (var tree in trees)
            {
                var context = new MigrationContext();
                context.Populate(compilation, tree);

                var ast = migration.Apply(tree, context);
                Console.WriteLine(ast.ToString());
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            watch.Stop();
            Console.WriteLine("Refactoring took:" + watch.ElapsedMilliseconds);
        }
    }
}