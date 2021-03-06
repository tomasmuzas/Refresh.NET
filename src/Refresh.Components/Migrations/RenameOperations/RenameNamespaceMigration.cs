﻿using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors.RenameOperations;

namespace Refresh.Components.Migrations.RenameOperations
{
    public class RenameNamespaceMigration : IMigration
    {
        private readonly string _namespaceName;
        private readonly string _newName;

        public RenameNamespaceMigration(string namespaceName, string newName)
        {
            _namespaceName = namespaceName;
            _newName = newName;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new RenameNamespaceVisitor(context, _namespaceName, _newName);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
