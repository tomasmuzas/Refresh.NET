﻿using LibAdapter.Components.Migrations;
using LibAdapter.Components.Migrations.ChangeOperations;
using LibAdapter.Components.Migrations.RenameOperations;
using Xunit;

namespace LibAdapterTests
{
    public class ChangeMethodReturnTypeMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_RenamesMethodInvocations_WithVariablesAsParameters()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public int TestMethod(){return 1;}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        var value = instance.TestMethod();
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public int TestMethod(){return 1;}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        object value = instance.TestMethod();
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new ChangeMethodReturnTypeMigration(
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "TestMethod"
                    },
                    "object"),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
