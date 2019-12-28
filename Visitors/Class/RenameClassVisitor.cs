using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.Class
{
    public class RenameClassVisitor : ClassVisitor
    {
        private string FullTypeName { get; }

        private string NewName { get; }

        public RenameClassVisitor(SyntaxTypeMap map, string fullTypeName, string newName) : base(map)
        {
            FullTypeName = fullTypeName;
            NewName = newName;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (MatchesClassType(node, FullTypeName))
            {
                var newNode = node.WithIdentifier(SyntaxFactory.Identifier(NewName)
                    .WithTrailingTrivia(node.Identifier.TrailingTrivia)
                    .WithLeadingTrivia(node.Identifier.LeadingTrivia));

                Map.UpdateIdentifierMap(node, newNode);

                node = newNode;
            }

            return node;
        }
    }
}
