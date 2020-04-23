using LibAdapter.Components.Migrations.RenameOperations;
using Xunit;

namespace LibAdapterTests
{
    public class RenameMemberMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_RenamesInstantiatedClass()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public string Member;
                }

                public class Program
                {
                    public string Main()
                    {
                        var instance = new TestClass();
                        instance.Member = ""hey"";
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public string Member;
                }

                public class Program
                {
                    public string Main()
                    {
                        var instance = new TestClass();
                        instance.NewMember = ""hey"";
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new RenameMemberMigration("Test.TestClass", "Member", "NewMember"), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
