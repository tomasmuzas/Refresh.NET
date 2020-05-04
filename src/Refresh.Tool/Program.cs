using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildalyzer;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Onion.SolutionParser.Parser;
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

            [Option('s', "Solution", Required = false, HelpText = "Solution to be refactored")]
            public string SolutionPath { get; set; }

            [Option('m', "Migration", Required = true, HelpText = "Path to migration file")]
            public string MigrationPath { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CliOptions>(args)
                .WithParsed(o =>
                {
                    if (string.IsNullOrEmpty(o.SolutionPath)
                        && string.IsNullOrEmpty(o.MigrationPath))
                    {
                        Console.WriteLine("Either project or sulution path must be provided.");
                        return;
                    }

                    var pathsToMigrate = new List<string>();

                    if (!string.IsNullOrEmpty(o.SolutionPath))
                    {
                        var solution = SolutionParser.Parse(o.SolutionPath);
                        var solutionDirectory = Path.GetDirectoryName(o.SolutionPath);
                        pathsToMigrate = solution.Projects
                            .Select(p => Path.Join(solutionDirectory, p.Path))
                            .ToList();
                    }
                    else
                    {
                        pathsToMigrate.Add(o.ProjectPath);
                    }

                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    foreach (var projectPath in pathsToMigrate)
                    {
                        Console.WriteLine($"Compiling project {projectPath}");
                        var manager = new AnalyzerManager();
                        var project = manager.GetProject(projectPath);
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

                        var migration = MigrationLoader.FromPath(o.MigrationPath);

                        foreach (var (tree, path) in trees)
                        {
                            Console.WriteLine("Running migration on " + path);

                            var context = new MigrationContext();
                            context.Populate(compilation, tree);

                            var ast = migration.Apply(tree, context);
                            File.WriteAllText(path, ast.ToString());
                        }
                    }
                    
                    watch.Stop();
                    Console.WriteLine("Refactoring took:" + watch.ElapsedMilliseconds);
                });
        }
    }
}