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

        public override SyntaxNode VisitGenericName(GenericNameSyntax node)
        {
            node = (GenericNameSyntax)base.VisitGenericName(node);

            var type = _context.GetNodeType(node);

            if (ConstructFullClassName(type) == ConstructFullClassName(Type)
                && TokenIsType(node.Identifier, type))
            {
                node = node.WithIdentifier(SyntaxFactory.Identifier(NewName)
                    .WithTrailingTrivia(node.Identifier.TrailingTrivia)
                    .WithLeadingTrivia(node.Identifier.LeadingTrivia));

                node = node.CopyAnnotationsTo(node);
            }

            return node;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            node = (IdentifierNameSyntax) base.VisitIdentifierName(node);

            if (_context.GetNodeType(node) == Type 
                && TokenIsType(node.Identifier, Type))
            {
                node = node.WithIdentifier(SyntaxFactory.Identifier(NewName)
                    .WithTrailingTrivia(node.Identifier.TrailingTrivia)
                    .WithLeadingTrivia(node.Identifier.LeadingTrivia));

                node = node.CopyAnnotationsTo(node);
            }

            return node;
        }

        private bool TokenIsType(SyntaxToken token, FullType type)
        {
            var name = token.WithoutTrivia().ToString();
            return name == type.ClassName
                   || name == Type;
        }

        private string ConstructFullClassName(FullType type)
        {
            if (type == (FullType) null)
            {
                return null;
            }

            return type.Namespace + "." + type.ClassName;
        }
    }
}
