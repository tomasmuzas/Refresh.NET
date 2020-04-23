using LibAdapter.Components.Migrations.Builders;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Components.Migrations
{
    public class CustomMigration : IMigration
    {
        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var ns = "LibAdapterTestSolution";
            var newClass = $"{ns}.NewClass";
            return new MigrationBuilder()
                .RenameClass($"{ns}.TestClass", "NewClass")
                .RenameMethod(m => m
                    .OfClass(newClass)
                    .WithName("TestMethod"), 
                    "NewMethod")
                .AddArguments(m => m
                    .OfClass(newClass)
                    .WithName("NewMethod"),
                    a => a
                        .OfType("string")
                        .WithDefaultValueExpression("\"value\"")
                        .AtPosition(1),
                    a => a
                        .OfType("string")
                        .AtPosition(2))
                .Build()
                .Apply(initialAST, context);
        }
    }
}
