using System.Collections.Generic;
using LibAdapter.Migrations;
using LibAdapter.Migrations.MethodSignatureOperations;
using Xunit;

namespace LibAdapterTests
{
    public class RemoveArgumentsMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_TransformsSourceCodeCorrectly()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(string something, int whatever, int num){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod(""Something"", 1, 2);
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(string something, int whatever, int num){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod(""Something"");
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new RemoveArgumentsMigration(
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "TestMethod"
                    },
                    new List<int>{2, 3}), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
