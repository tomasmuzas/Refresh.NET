using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;

namespace Refresh.Components.Visitors.RenameOperations
{
    public class RenameMemberVisitor : CSharpSyntaxRewriter
    {
        private readonly MigrationContext _context;
        private readonly string _type;
        private readonly string _memberName;
        private readonly string _newName;

        public RenameMemberVisitor(
            MigrationContext context,
            string type,
            string memberName,
            string newName)
        {
            _context = context;
            _type = type;
            _memberName = memberName;
            _newName = newName;
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            node = (MemberAccessExpressionSyntax) base.VisitMemberAccessExpression(node);

            var type = _context.GetNodeContainingClassType(node);

            if (type == _type && node.Name.ToString() == _memberName)
            {
                node = node
                    .WithName(SyntaxFactory.IdentifierName(_newName))
                    .WithLeadingTrivia(node.GetLeadingTrivia())
                    .WithTrailingTrivia(node.GetTrailingTrivia());

                node = node.CopyAnnotationsTo(node);
            }

            return node;
        }
    }
}
