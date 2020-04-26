using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;

namespace Refresh.Components.Visitors.ChangeOperations
{
    public class ChangeMemberTypeVisitor : CSharpSyntaxRewriter
    {
        private readonly MigrationContext _context;
        private readonly string _type;
        private readonly string _memberName;
        private readonly string _returnType;

        public ChangeMemberTypeVisitor(
            MigrationContext context,
            string type,
            string memberName,
            string returnType)
        {
            _context = context;
            _type = type;
            _memberName = memberName;
            _returnType = returnType;
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            node = (LocalDeclarationStatementSyntax) base.VisitLocalDeclarationStatement(node);

            var initializer = node.Declaration
                .DescendantNodes()
                .OfType<EqualsValueClauseSyntax>()
                .FirstOrDefault();

            var memberAccess = initializer?
                .DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .FirstOrDefault();

            if (memberAccess != null
                && _context.GetNodeContainingClassType(memberAccess) == _type
                && memberAccess.Name.ToString() == _memberName)
            {
                var newMemberAccess = node.Declaration
                    .WithType(
                        SyntaxFactory.ParseTypeName(_returnType)
                            .WithTriviaFrom(node.Declaration.Type));

                node = node.ReplaceNode(node.Declaration, newMemberAccess);

                _context.UpdateNodeType(newMemberAccess, _returnType);
            }

            return node;
        }
    }
}
