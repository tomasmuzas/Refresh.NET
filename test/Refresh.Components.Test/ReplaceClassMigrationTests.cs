using Refresh.Components.Migrations;
using Refresh.Components.Migrations.ReplaceOperations;
using Xunit;

namespace Refresh.Components.Test
{
    public class ReplaceClassMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_ReplacesConstructorArguments()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                }

                public class Program
                {
                    public void Main()
                    {
                        OtherNamespace.NewClass instance = new OtherNamespace.NewClass(new object());
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new ReplaceClassMigration(
                    "Test.TestClass",
                    "OtherNamespace.NewClass",
                    new [] { 
                        new Argument
                        {
                            Type = "object",
                            DefaultValueExpression = "new object()"
                        }}),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }

        [Fact]
        public void Apply_ReplacesClassTypedVariable()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                }

                public class Program
                {
                    TestClass instance;
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                }

                public class Program
                {
                    Test.NewClass instance;
                }
            }";

            var refactoredAst = PerformMigration(
                new ReplaceClassMigration(
                    "Test.TestClass", 
                    "Test.NewClass",
                    new Argument[]{}),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
