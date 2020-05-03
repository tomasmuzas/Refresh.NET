using Refresh.Components.Migrations.RenameOperations;
using Xunit;

namespace Refresh.Components.Test
{
    public class RenameClassMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_RenamesInstantiatedClass()
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
                        var instance = new NewClass();
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameClassMigration("Test.TestClass", "NewClass"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }

        [Fact]
        public void Apply_RenamesClassTypedVariable()
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
                    NewClass instance;
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameClassMigration("Test.TestClass", "NewClass"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }

        [Fact]
        public void Apply_RenamesGenericInstantiatedClass()
        {
            var source = @"
            namespace Test
            {
                public class TestClass<T>
                {
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass<string>();
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass<T>
                {
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new NewClass<string>();
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameClassMigration("Test.TestClass", "NewClass"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }

        [Fact]
        public void Apply_RenamesGenericClassTypedVariable()
        {
            var source = @"
            namespace Test
            {
                public class TestClass<T>
                {
                }

                public class Program
                {
                    TestClass<string> instance;
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass<T>
                {
                }

                public class Program
                {
                    NewClass<string> instance;
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameClassMigration("Test.TestClass", "NewClass"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }

        [Fact]
        public void Apply_RenamesOnlyTypes_WhenTheClassIsParameter()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public void Do(){}
                }

                public class Program
                {
                    public void DoSomething(TestClass test)
                    {
                        test.Do();
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public void Do(){}
                }

                public class Program
                {
                    public void DoSomething(NewClass test)
                    {
                        test.Do();
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameClassMigration("Test.TestClass", "NewClass"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}