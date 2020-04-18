using System.Collections.Generic;
using LibAdapter.Migrations;
using Xunit;

namespace LibAdapterTests
{
    public class AddArgumentMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_TransformsSourceCodeCorrectly()
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
                        instance.TestMethod(new object());
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new AddArgumentsMigration(
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "TestMethod"
                    },
                    new List<(Argument argument, int position)>
                    {
                        (
                        new Argument
                        {
                            DefaultValueExpression = "new object()"
                        },
                        1)
                    }), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
