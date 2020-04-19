using System.Collections.Generic;
using LibAdapter.Migrations;
using LibAdapter.Migrations.MethodSignatureOperations;
using Xunit;

namespace LibAdapterTests
{
    public class ChangeArgumentsMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_TransformsSourceCodeCorrectly()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(object obj){}
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

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(object obj){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod(new string());
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new ChangeArgumentsMigration(
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
                                DefaultValueExpression = "new string()"
                            },
                            1)
                    }), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
