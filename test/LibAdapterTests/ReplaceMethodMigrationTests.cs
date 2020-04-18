using System.Collections.Generic;
using LibAdapter.Migrations;
using LibAdapter.Migrations.MethodSignatureOperations;
using LibAdapter.Migrations.ReplaceOperations;
using Xunit;

namespace LibAdapterTests
{
    public class ReplaceMethodMigrationTests : MigrationTestBase
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
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new ReplaceMethodMigration(
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "TestMethod",
                        Arguments = new[] {new Argument { Type = "string"}, new Argument { Type = "int" }, new Argument { Type = "string" } }
                    },
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "OtherMethod",
                        Arguments = new[] { new Argument { Type = "string" }, new Argument { Type = "int" } }
                    }), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
