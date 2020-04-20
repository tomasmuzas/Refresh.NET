using LibAdapter.Migrations.Builders;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations
{
    public class CustomMigration : IMigration
    {
        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            return new MigrationBuilder()
                .RenameClass("LibAdapterTestSolution.TestClass", "NewClass")
                .RenameMethod(m => m
                    .OfClass("LibAdapterTestSolution.NewClass")
                    .WithName("TestMethod"), 
                    "NewMethod")
                .Build()
                .Apply(initialAST, context);;
        }
    }
}
