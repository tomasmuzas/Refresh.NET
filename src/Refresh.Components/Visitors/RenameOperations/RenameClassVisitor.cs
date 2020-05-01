using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;

namespace Refresh.Components.Visitors.RenameOperations
{
    public class RenameClassVisitor : CSharpSyntaxRewriter
    {
        private readonly MigrationContext _context;
        private FullType Type { get; }
        private string NewName { get; }

        public RenameClassVisitor(MigrationContext context, FullType type, string newName)
        {
            _context = context;
            Type = type;
            NewName = newName;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            node = (IdentifierNameSyntax) base.VisitIdentifierName(node);

            if (_context.GetNodeType(node) == Type 
                && IdentifierIsType(node, Type))
            {
                node = node.WithIdentifier(SyntaxFactory.Identifier(NewName)
                    .WithTrailingTrivia(node.Identifier.TrailingTrivia)
                    .WithLeadingTrivia(node.Identifier.LeadingTrivia));

                node = node.CopyAnnotationsTo(node);
            }

            return node;
        }

        private bool IdentifierIsType(IdentifierNameSyntax identifier, FullType type)
        {
            var name = identifier.Identifier.WithoutTrivia().ToString();
            return name == type.ClassName
                   || name == Type
                   || name == "var";

        }
    }
}
