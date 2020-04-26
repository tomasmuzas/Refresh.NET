using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Buildalyzer;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Refresh.Components.Migrations;
using Refresh.Components.Visitors;

namespace Refresh.Tool 
{
    class Program
    {
        private class CliOptions
        {
            [Option('p', "Project", Required=true, HelpText = "Project to be refactored")]
            public string ProjectPath { get; set; }

            [Option('m', "Migration", Required = true, HelpText = "Path to migration file")]
            public string MigrationPath { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CliOptions>(args)
                .WithParsed(o =>
                {
                    Console.WriteLine("Compiling project");
                    var manager = new AnalyzerManager();
                    var project = manager.GetProject(o.ProjectPath);
                    var result = project.Build().Results.ElementAt(0);

                    if (!result.Succeeded)
                    {
                        Console.WriteLine("Failed to build a project.");
                        return;
                    }

                    var references = result.References;
                    var sourceFiles = result.SourceFiles;

                    var compilation = CSharpCompilation.Create("Code")
                        .AddReferences(references.Select(s => MetadataReference.CreateFromFile(s)));

                    var trees = new List<(SyntaxTree tree, string path)>();
                    foreach (var filePath in sourceFiles)
                    {
                        var code = File.ReadAllText(filePath);
                        var tree = CSharpSyntaxTree.ParseText(code);
                        tree = tree.WithRootAndOptions(new AnnotationVisitor().Visit(tree.GetRoot()), tree.Options);

                        trees.Add((tree, filePath));
                    }

                    compilation = compilation.AddSyntaxTrees(trees.Select(t => t.tree));

                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    var migration = MigrationLoader.FromPath(o.MigrationPath);

                    foreach (var (tree, path) in trees)
                    {
                        Console.WriteLine("Running migration on " + path);

                        var context = new MigrationContext();
                        context.Populate(compilation, tree);

                        var ast = migration.Apply(tree, context);
                        var file = File.OpenWrite(path);
                        file.Write(Encoding.UTF8.GetBytes((string) ast.ToString()));
                        file.Flush();
                    }

                    watch.Stop();
                    Console.WriteLine("Refactoring took:" + watch.ElapsedMilliseconds);
                });
        }
    }
}