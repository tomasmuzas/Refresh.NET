using LibAdapter.Migrations.RenameOperations;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations
{
    public class CustomMigration : IMigration
    {
        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            return new CompositeMigration(
                new RenameClassMigration("LibAdapterTestSolution.TestClass", "NewClass"),
                new RenameMethodMigration(new Method{Type= "LibAdapterTestSolution.NewClass", Name = "TestMethod" }, "NewMethod")
                )
                .Apply(initialAST, context);
        }
    }
}
