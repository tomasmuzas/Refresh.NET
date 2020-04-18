using LibAdapter.Migrations;
using LibAdapter.Migrations.RenameOperations;
using Xunit;

namespace LibAdapterTests
{
    public class CompositeMigrationTest : MigrationTestBase
    {
        [Fact]
        public void Apply_AppliesAllMigrations_InOrder()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod();
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(){}
                }

                public class Program
                {
                    public void Main()
                    {
                        NewClass instance = new NewClass();
                        instance.NewMethod();
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new CompositeMigration(
                        new RenameClassMigration("Test.TestClass", "NewClass"),
                        new RenameMethodMigration(
                            new Method
                            {
                                Type = "Test.TestClass",
                                Name = "TestMethod"
                            },
                            "NewMethod")),
                    source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
