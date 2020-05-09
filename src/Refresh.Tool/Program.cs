using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Buildalyzer.Workspaces;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
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

            [Option('v', "Verbose", Required = false, HelpText = "Enable verbose logging")]
            public bool Verbose { get; set; }
        }

        static async Task Main(string[] args)
        {
            var parsedArguments = Parser.Default.ParseArguments<CliOptions>(args);

            ILogger<Program> logger = new Logger<Program>(
                new LoggerFactory());

            await parsedArguments.MapResult(
                async o => await PerformMigration(o),
                errors => Task.CompletedTask);
        }

        private static async Task PerformMigration(CliOptions options)
        {
            if (string.IsNullOrEmpty(options.SolutionPath)
                && string.IsNullOrEmpty(options.MigrationPath))
            {
                Console.WriteLine("Either project or sulution path must be provided.");
                return;
            }

            var workspace = new AdhocWorkspace();

            if (!string.IsNullOrEmpty(options.SolutionPath))
            {
                Console.WriteLine("Loading solution:");
                var manager = new AnalyzerManager(options.SolutionPath);
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
                manager.GetProject(options.ProjectPath).AddToWorkspace(workspace);
            }

            Console.WriteLine("Loading migration file");
            var migration = MigrationLoader.FromPath(options.MigrationPath);

            var timer = Stopwatch.StartNew();
            await Task.WhenAll(workspace.CurrentSolution.Projects.Select(async project =>
            {
                Console.WriteLine($"Migrating project {project.Name}");

                var compilation = (CSharpCompilation) await project.GetCompilationAsync();

                await Task.WhenAll(compilation.SyntaxTrees.Select(async tree =>
                {
                    var annotatedTree = tree.WithRootAndOptions(
                        new AnnotationVisitor().Visit(await tree.GetRootAsync()),
                        tree.Options);

                    compilation = compilation.ReplaceSyntaxTree(tree, annotatedTree);

                    if (options.Verbose)
                    {
                        Console.WriteLine("Running migration on " + tree.FilePath);
                    }

                    var context = new MigrationContext();
                    context.Populate(compilation, annotatedTree);

                    var ast = migration.Apply(annotatedTree, context);

                    if (ast != annotatedTree)
                    {
                        await File.WriteAllTextAsync(tree.FilePath, ast.ToString());
                    }
                }));
            }));

            Console.WriteLine("Migration took: " + timer.Elapsed);
        }
    }
}