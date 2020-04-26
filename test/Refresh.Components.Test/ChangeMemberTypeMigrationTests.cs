using Refresh.Components.Migrations;
using Refresh.Components.Migrations.ChangeOperations;
using Xunit;

namespace Refresh.Components.Test
{
    public class ChangeMemberTypeMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_RenamesMethodInvocations_WithVariablesAsParameters()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public int TestMember;
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        var value = instance.TestMember;
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public int TestMember;
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        string value = instance.TestMember;
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new ChangeMemberTypeMigration(
                    "Test.TestClass",
                    "TestMember",
                    "string"), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
