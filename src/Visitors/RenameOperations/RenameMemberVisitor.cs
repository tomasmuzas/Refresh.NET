using LibAdapter.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.RenameOperations
{
    public class RenameMemberVisitor : ClassVisitor
    {
        private readonly string _type;
        private readonly string _memberName;
        private readonly string _newName;

        public RenameMemberVisitor(
            MigrationContext context,
            string type,
            string memberName,
            string newName) : base(context)
        {
            _type = type;
            _memberName = memberName;
            _newName = newName;
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            node = (MemberAccessExpressionSyntax) base.VisitMemberAccessExpression(node);

            var type = Context.GetNodeContainingClassType(node);

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
