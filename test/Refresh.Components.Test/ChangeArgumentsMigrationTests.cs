﻿using System.Collections.Generic;
using Refresh.Components.Migrations;
using Refresh.Components.Migrations.MethodSignatureOperations;
using Xunit;

namespace Refresh.Components.Test
{
    public class ChangeArgumentsMigrationTests : MigrationTestBase
    {
        [Fact]
        public void Apply_ReplacesMethodArgument()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(object obj){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod(new object());
                    }
                }
            }";

            var expected = @"
            namespace Test
            {
                public class TestClass
                {
                    public void TestMethod(object obj){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod(new string());
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new ChangeArgumentsMigration(
                    new Method
                    {
                        Type = "Test.TestClass",
                        Name = "TestMethod"
                    },
                    new List<PositionalArgument>
                    {
                        new PositionalArgument
                        {
                            DefaultValueExpression = "new string()",
                            Position = 1
                        }
                    }), 
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }

        [Fact]
        public void Apply_HandlesExtensionMethodArguments()
        {
            var source = @"
            namespace Test
            {
                public class TestClass
                {
                }

                public static class TestClassExtensions
                {
                    public static void TestMethod(this TestClass obj, object o){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod(new object());
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
                    public static void TestMethod(this TestClass obj, object o){}
                }

                public class Program
                {
                    public void Main()
                    {
                        var instance = new TestClass();
                        instance.TestMethod(new string());
                    }
                }
            }";

            var refactoredAst = PerformMigration(
                new ChangeArgumentsMigration(
                    new Method
                    {
                        Type = "Test.TestClassExtensions",
                        Name = "TestMethod"
                    },
                    new List<PositionalArgument>
                    {
                        new PositionalArgument
                        {
                            DefaultValueExpression = "new string()",
                            Position = 1
                        }
                    }),
                source);

            Assert.Equal(expected, refactoredAst.ToString());
        }
    }
}
