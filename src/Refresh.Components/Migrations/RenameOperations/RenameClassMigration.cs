﻿using System.Linq;
using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors.RenameOperations;

namespace Refresh.Components.Migrations.RenameOperations
{
    public class RenameClassMigration : IMigration
    {
        private readonly FullType _type;
        private readonly string _newName;

        public RenameClassMigration(FullType type, string newName)
        {
            _type = type;
            _newName = newName;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new RenameClassVisitor(context, _type, _newName);
            var newAst = visitor.Visit(initialAST.GetRoot());

            var newType = _type.Namespace + "." + _newName;

            context.ReplaceType(_type, newType);

            return newAst.SyntaxTree;
        }
    }
}
