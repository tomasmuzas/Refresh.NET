using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandLine;
using LibAdapter.Components.Migrations;
using LibAdapter.Components.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Tool 
{
    class Program
    {
        private class CliOptions
        {
            [Option('p', "Project", Required=true, HelpText = "Project to be refactored")]
            public string ProjectPath { get; set; }

            [Option('d', "MainDllPath", Required = true, HelpText = "Path to main DLL")]
            public string MainDllPath { get; set; }

            [Option('m', "Migration", Required = true, HelpText = "Path to migration file")]
            public string MigrationPath { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CliOptions>(args)
                .WithParsed(o =>
                {
                    var references = Assembly.LoadFile(o.MainDllPath).GetReferencedAssemblies();

                    var compilation = CSharpCompilation.Create("Code")
                        .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                        .AddReferences(MetadataReference.CreateFromFile(o.MainDllPath));

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
                    foreach (var filePath in Directory.GetFiles(o.ProjectPath, "*.cs", SearchOption.AllDirectories))
                    {
                        var code = File.ReadAllText(filePath);
                        var tree = CSharpSyntaxTree.ParseText(code);
                        tree = tree.WithRootAndOptions(new AnnotationVisitor().Visit(tree.GetRoot()), tree.Options);

                        trees.Add(tree);
                    }

                    compilation = compilation.AddSyntaxTrees(trees);

                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    var migration = MigrationLoader.FromPath(o.MigrationPath);

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
                });
        }
    }
}