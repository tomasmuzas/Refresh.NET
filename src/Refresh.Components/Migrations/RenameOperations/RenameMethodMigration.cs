﻿using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors.RenameOperations;

namespace Refresh.Components.Migrations.RenameOperations
{
    public class RenameMethodMigration : IMigration
    {
        private readonly Method _method;
        private readonly string _newName;

        public RenameMethodMigration(Method method, string newName)
        {
            _method = method;
            _newName = newName;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new RenameMethodVisitor(context, _method, _newName);
            var newAst = visitor.Visit(initialAST.GetRoot());

            return newAst.SyntaxTree;
        }
    }
}
