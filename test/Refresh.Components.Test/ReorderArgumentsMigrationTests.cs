using System.Collections.Generic;
using LibAdapter.Components.Migrations;
using LibAdapter.Components.Migrations.MethodSignatureOperations;
using Xunit;

namespace LibAdapterTests
{
    public class ReorderArgumentsMigrationTests : MigrationTestBase
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
                        instance.TestMethod(""test"",1,""name"");
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new ReorderArgumentsMigration(
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "TestMethod"
                    },
                    new List<int> {3, 2, 1}), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
