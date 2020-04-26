using LibAdapter.Components.Migrations.RenameOperations;
using Xunit;

namespace LibAdapterTests
{
    public class RenameNamespaceMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_RenamesNamespaceInUsing()
        {
            var source = @"
            using Test;
            namespace Test
            {
            }";

            var expected = @"
            using NewNamespace;
            namespace Test
            {
            }";

            var refactoredAst = PerformMigration(
                new RenameNamespaceMigration("Test", "NewNamespace"), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
