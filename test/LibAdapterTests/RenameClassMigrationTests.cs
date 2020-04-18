using LibAdapter.Migrations;
using Xunit;

namespace LibAdapterTests
{
    public class RenameClassMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_TransformsSourceCodeCorrectly()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                }

                public class Program
                {
                    var instance = new TestClass();
                }
            }
            ";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                }

                public class Program
                {
                    var instance = new NewClass();
                }
            }
            ";

            var ast = PerformMigration(
                new RenameClassMigration("Test.TestClass", "NewClass"),
                source);

            Assert.Equal(expected, ast.ToString());
        }
    }
}