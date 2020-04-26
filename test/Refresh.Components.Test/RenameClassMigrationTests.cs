﻿using LibAdapter.Components.Migrations.RenameOperations;
using Xunit;

namespace LibAdapterTests
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
                        NewClass instance = new NewClass();
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
    }
}