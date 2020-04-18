using LibAdapter.Migrations;
using LibAdapter.Migrations.RenameOperations;
using Xunit;

namespace LibAdapterTests
{
    public class RenameMethodMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_RenamesMethodInvocations()
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
                        var instance = new TestClass();
                        instance.NewMethod();
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameMethodMigration(
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "TestMethod"
                    }, 
                    "NewMethod"), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
