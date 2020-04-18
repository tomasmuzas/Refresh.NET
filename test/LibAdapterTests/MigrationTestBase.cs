using LibAdapter.Migrations;
using LibAdapter.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapterTests
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