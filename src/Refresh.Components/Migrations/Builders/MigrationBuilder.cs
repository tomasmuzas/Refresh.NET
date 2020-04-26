using System;
using System.Collections.Generic;
using LibAdapter.Components.Migrations.MethodSignatureOperations;
using LibAdapter.Components.Migrations.RenameOperations;
using LibAdapter.Components.Migrations.ReplaceOperations;

namespace LibAdapter.Components.Migrations.Builders
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

        public MigrationBuilder AddArguments(Action<MethodBuilder> methodSetup, params Action<PositionalArgumentBuilder>[] argumentSetups)
        {
            var methodBuilder = new MethodBuilder();
            methodSetup.Invoke(methodBuilder);
            var method = methodBuilder.Build();

            var arguments = new List<PositionalArgument>();
            foreach (var action in argumentSetups)
            {
                var builder = new PositionalArgumentBuilder();
                action.Invoke(builder);
                var argument = builder.Build();
                arguments.Add(argument);
            }

            _migration.AddMigration(new AddArgumentsMigration(method, arguments));
            return this;
        }

        public MigrationBuilder ChangeArguments(Method method, List<PositionalArgument> arguments)
        {
            _migration.AddMigration(new ChangeArgumentsMigration(method, arguments));
            return this;
        }

        public MigrationBuilder ChangeArguments(Action<MethodBuilder> methodSetup, List<PositionalArgument> arguments)
        {
            var methodBuilder = new MethodBuilder();
            methodSetup.Invoke(methodBuilder);
            var method = methodBuilder.Build();
            _migration.AddMigration(new ChangeArgumentsMigration(method, arguments));
            return this;
        }

        public MigrationBuilder ChangeArguments(Action<MethodBuilder> methodSetup, params Action<PositionalArgumentBuilder>[] argumentSetups)
        {
            var methodBuilder = new MethodBuilder();
            methodSetup.Invoke(methodBuilder);
            var method = methodBuilder.Build();

            var arguments = new List<PositionalArgument>();
            foreach (var action in argumentSetups)
            {
                var builder = new PositionalArgumentBuilder();
                action.Invoke(builder);
                var argument = builder.Build();
                arguments.Add(argument);
            }

            _migration.AddMigration(new ChangeArgumentsMigration(method, arguments));
            return this;
        }

        public MigrationBuilder RemoveArguments(Method method, List<int> positions)
        {
            _migration.AddMigration(new RemoveArgumentsMigration(method, positions));
            return this;
        }

        public MigrationBuilder RemoveArguments(Action<MethodBuilder> methodSetup, List<int> positions)
        {
            var methodBuilder = new MethodBuilder();
            methodSetup.Invoke(methodBuilder);
            var method = methodBuilder.Build();
            _migration.AddMigration(new RemoveArgumentsMigration(method, positions));
            return this;
        }

        public MigrationBuilder ReorderArguments(Method method, List<int> positions)
        {
            _migration.AddMigration(new ReorderArgumentsMigration(method, positions));
            return this;
        }

        public MigrationBuilder ReorderArguments(Action<MethodBuilder> methodSetup, List<int> positions)
        {
            var methodBuilder = new MethodBuilder();
            methodSetup.Invoke(methodBuilder);
            var method = methodBuilder.Build();
            _migration.AddMigration(new ReorderArgumentsMigration(method, positions));
            return this;
        }


        public IMigration Build()
        {
            return _migration;
        }
    }
}
