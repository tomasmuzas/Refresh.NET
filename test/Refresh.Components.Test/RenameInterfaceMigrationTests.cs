using Refresh.Components.Migrations.RenameOperations;
using Xunit;

namespace Refresh.Components.Test
{
    public class RenameInterfaceMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_RenamesInheritedInterface()
        {
            var source = @"
            namespace Test
            {
                public interface ITestInterface{}

                public class TestClass : ITestInterface
                {
                }

                public class Program
                {
                    var instance = new TestClass();
                }
            }";

            var expected = @"
            namespace Test
            {
                public interface ITestInterface{}

                public class TestClass : INewInterface
                {
                }

                public class Program
                {
                    var instance = new TestClass();
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameInterfaceMigration("Test.ITestInterface", "INewInterface"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }

        [Fact]
        public void Apply_RenamesInterfaceTypedVariable()
        {
            var source = @"
            namespace Test
            {
                public interface ITestInterface{}

                public class Program
                {
                    ITestInterface instance;
                }
            }";

            var expected = @"
            namespace Test
            {
                public interface ITestInterface{}

                public class Program
                {
                    INewInterface instance;
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameInterfaceMigration("Test.ITestInterface", "INewInterface"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
