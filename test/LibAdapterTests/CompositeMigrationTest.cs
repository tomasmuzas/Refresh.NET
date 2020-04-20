﻿using System.Collections.Generic;
using LibAdapter.Migrations;
using LibAdapter.Migrations.MethodSignatureOperations;
using LibAdapter.Migrations.RenameOperations;
using Xunit;

namespace LibAdapterTests
{
    public class CompositeMigrationTest : MigrationTestBase
    {
        [Fact]
        public void Apply_AppliesAllMigrations_InOrder()
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
                        NewClass instance = new NewClass();
                        instance.NewMethod(new object());
                    }
                }
            }";

            var newMethod = new Method
            {
                Type = "Test.NewClass",
                Name = "NewMethod"
            };
            var refactoredAst = PerformMigration(
                new CompositeMigration(
                        new RenameClassMigration("Test.TestClass", "NewClass"),
                        new RenameMethodMigration(
                            new Method
                            {
                                Type = "Test.NewClass",
                                Name = "TestMethod"
                            }, 
                            "NewMethod"),
                        new AddArgumentsMigration(
                            newMethod, 
                            new List<PositionalArgument>
                            {
                                new PositionalArgument
                                {
                                    DefaultValueExpression = "new object()",
                                    Position = 1
                                },
                                new PositionalArgument
                                {
                                    DefaultValueExpression = "new string()",
                                    Position = 2
                                }
                            }),
                        new RemoveArgumentsMigration(newMethod, new []{2})),
                    source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
