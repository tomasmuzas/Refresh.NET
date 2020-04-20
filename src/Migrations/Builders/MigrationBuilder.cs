using System;
using System.Collections.Generic;
using LibAdapter.Migrations.MethodSignatureOperations;
using LibAdapter.Migrations.RenameOperations;
using LibAdapter.Migrations.ReplaceOperations;

namespace LibAdapter.Migrations.Builders
{
    public class MigrationBuilder
    {
        private readonly CompositeMigration _migration = new CompositeMigration();

        public MigrationBuilder RenameClass(string type, string newName)
        {
            _migration.AddMigration(new RenameClassMigration(type, newName));
            return this;
        }

        public MigrationBuilder RenameInterface(string type, string newName)
        {
            _migration.AddMigration(new RenameInterfaceMigration(type, newName));
            return this;
        }

        public MigrationBuilder RenameMember(string type, string memberName, string newName)
        {
            _migration.AddMigration(new RenameMemberMigration(type, memberName,newName));
            return this;
        }

        public MigrationBuilder RenameNamespace(string namespaceName, string newName)
        {
            _migration.AddMigration(new RenameNamespaceMigration(namespaceName, newName));
            return this;
        }

        public MigrationBuilder RenameMethod(Method method, string newName)
        {
            _migration.AddMigration(new RenameMethodMigration(method, newName));
            return this;
        }

        public MigrationBuilder RenameMethod(Action<MethodBuilder> methodSetup, string newName)
        {
            var methodBuilder = new MethodBuilder();
            methodSetup.Invoke(methodBuilder);
            var method = methodBuilder.Build();
            _migration.AddMigration(new RenameMethodMigration(method, newName));
            return this;
        }

        public MigrationBuilder ReplaceClass(string type, string newType)
        {
            _migration.AddMigration(new RenameClassMigration(type, newType));
            return this;
        }

        public MigrationBuilder ReplaceMethod(Method method, Method newMethod)
        {
            _migration.AddMigration(new ReplaceMethodMigration(method, newMethod));
            return this;
        }

        public MigrationBuilder AddArguments(Method method, List<PositionalArgument> arguments)
        {
            _migration.AddMigration(new AddArgumentsMigration(method, arguments));
            return this;
        }

        public MigrationBuilder AddArguments(Action<MethodBuilder> methodSetup, List<PositionalArgument> arguments)
        {
            var methodBuilder = new MethodBuilder();
            methodSetup.Invoke(methodBuilder);
            var method = methodBuilder.Build();
            _migration.AddMigration(new AddArgumentsMigration(method, arguments));
            return this;
        }

        public IMigration Build()
        {
            return _migration;
        }
    }
}
