using Refresh.Components.Migrations;
using Refresh.Components.Migrations.RenameOperations;
using Xunit;

namespace Refresh.Components.Test
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

        [Fact]
        public void Apply_RenamesExtensionMethodInvocations()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                }

                public static class TestClassExtensions
                {
                    public static void TestMethod(this TestClass obj){}
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
                }

                public static class TestClassExtensions
                {
                    public static void TestMethod(this TestClass obj){}
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
                        Type = "Test.TestClassExtensions",
                        Name = "TestMethod"
                    },
                    "NewMethod"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }

        [Fact]
        public void Apply_RenamesMethodInvocations_WithVariablesAsParameters()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(string value){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var str = ""Something""
                        var instance = new TestClass();
                        instance.TestMethod(str);
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(string value){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var str = ""Something""
                        var instance = new TestClass();
                        instance.NewMethod(str);
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
