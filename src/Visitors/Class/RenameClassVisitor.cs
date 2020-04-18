using LibAdapter.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.Class
{
    public class RenameClassVisitor : ClassVisitor
    {
        private string FullTypeName { get; }

        private string NewName { get; }

        public RenameClassVisitor(MigrationContext context, string fullTypeName, string newName) : base(context)
        {
            FullTypeName = fullTypeName;
            NewName = newName;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            node = (IdentifierNameSyntax) base.VisitIdentifierName(node);
            if (node != null && MatchesClassType(node, FullTypeName))
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
