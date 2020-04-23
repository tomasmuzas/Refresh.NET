using LibAdapter.Components.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Components.Visitors.RenameOperations
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
            }

            return node;
        }
    }
}
