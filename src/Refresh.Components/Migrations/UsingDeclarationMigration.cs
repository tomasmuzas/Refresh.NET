using System;
using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors;

namespace Refresh.Components.Migrations
{
    public class UsingDeclarationMigration : IMigration
    {
        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new UsingDeclarationVisitor(context);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
