using System.Linq;
using LibAdapter.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.RenameOperations
{
    public class RenameClassVisitor : CSharpSyntaxRewriter
    {
        private readonly MigrationContext _context;
        private string FullTypeName { get; }
        private string NewName { get; }

        public RenameClassVisitor(MigrationContext context, string fullTypeName, string newName)
        {
            _context = context;
            FullTypeName = fullTypeName;
            NewName = newName;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            node = (IdentifierNameSyntax) base.VisitIdentifierName(node);

            if (_context.GetNodeType(node) == FullTypeName)
            {
                node = node.WithIdentifier(SyntaxFactory.Identifier(NewName)
                    .WithTrailingTrivia(node.Identifier.TrailingTrivia)
                    .WithLeadingTrivia(node.Identifier.LeadingTrivia));

                node = node.CopyAnnotationsTo(node);

                var parts = FullTypeName.Split(".");
                var ns = string.Join(".", parts.Take(parts.Length - 1));
                var newType = ns + "." + NewName;

                _context.UpdateNodeContainingClassType(node, newType);
            }

            return node;
        }
    }
}
