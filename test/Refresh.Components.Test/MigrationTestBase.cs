using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Refresh.Components.Migrations;
using Refresh.Components.Visitors;

namespace Refresh.Components.Test
{
    public class MigrationTestBase
    {
        public SyntaxTree PerformMigration(IMigration migration, string sourceCode)
        {
            var compilation = CSharpCompilation.Create("Code")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            var tree = CSharpSyntaxTree.ParseText(sourceCode);
            tree = tree.WithRootAndOptions(new AnnotationVisitor().Visit(tree.GetRoot()), tree.Options);

            compilation = compilation.AddSyntaxTrees(tree);

            var context = new MigrationContext();
            context.Populate(compilation, tree);

            return migration.Apply(tree, context);
        }
    }
}