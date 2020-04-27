using Refresh.Components.Migrations;
using Refresh.Components.Migrations.ReplaceOperations;
using Refresh.Components.Migrations.SplitOperations;
using Xunit;

namespace Refresh.Components.Test
{
    public class SplitMethodMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_TransformsSourceCodeCorrectly()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(string name, int age, string something){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod(""name"", 1, ""test"");
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(string name, int age, string something){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.OtherMethod(""test"",1);
                        instance.Clean();
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new SplitMethodMigration(
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "TestMethod",
                        Arguments = new[] { new Argument { Type = "string" }, new Argument { Type = "int" }, new Argument { Type = "string" } }
                    },
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "OtherMethod",
                        Arguments = new[]
                        {
                            new Argument
                            {
                                Type = "string",
                                DefaultValueExpression = "\"test\""
                            },
                            new Argument
                            {
                                Type = "int",
                                DefaultValueExpression = "1"
                            }
                        }
                    },
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "Clean",
                        Arguments = new Argument[]{}
                    }), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
