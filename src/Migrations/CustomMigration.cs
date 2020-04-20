using System.Collections.Generic;
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
                .AddArguments(m => m
                    .OfClass("LibAdapterTestSolution.NewClass")
                    .WithName("NewMethod"),
                    new List<PositionalArgument>
                    {
                        new ArgumentBuilder()
                            .WithType("string")
                            .WithDefaultValueExpression("\"value\"")
                            .WithPosition(1)
                    })
                .Build()
                .Apply(initialAST, context);;
        }
    }
}
