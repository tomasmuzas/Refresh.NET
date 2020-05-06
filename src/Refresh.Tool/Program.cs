using System;
using System.IO;
using Buildalyzer;
using Buildalyzer.Workspaces;
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
            [Option('p', "Project", Required=false, HelpText = "Project to be refactored")]
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

                    var workspace = new AdhocWorkspace();

                    if (!string.IsNullOrEmpty(o.SolutionPath))
                    {
                        Console.WriteLine("Loading solution:");
                        var manager = new AnalyzerManager(o.SolutionPath);
                        foreach (var p in manager.Projects.Values)
                        {
                            Console.WriteLine($"Loading project: {p.ProjectFile.Path}");
                            p.AddToWorkspace(workspace);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Loading project");
                        var manager = new AnalyzerManager();
                        manager.GetProject(o.ProjectPath).AddToWorkspace(workspace);
                    }

                    Console.WriteLine("Loading migration file");
                    var migration = MigrationLoader.FromPath(o.MigrationPath);

                    foreach (var project in workspace.CurrentSolution.Projects)
                    {
                        Console.WriteLine($"Migrating project {project.Name}");

                        var compilation = (CSharpCompilation) project.GetCompilationAsync().Result;

                        foreach (var tree in compilation.SyntaxTrees)
                        {
                            var annotatedTree = tree.WithRootAndOptions(new AnnotationVisitor().Visit(tree.GetRoot()), tree.Options);
                            compilation = compilation.ReplaceSyntaxTree(tree, annotatedTree);

                            Console.WriteLine("Running migration on " + tree.FilePath);

                            var context = new MigrationContext();
                            context.Populate(compilation, annotatedTree);

                            var ast = migration.Apply(annotatedTree, context);
                            File.WriteAllText(tree.FilePath, ast.ToString());
                        }
                    }
                });
        }
    }
}