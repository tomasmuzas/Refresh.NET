using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;

namespace Refresh.Components.Visitors.ChangeOperations
{
    public class ChangeMethodReturnTypeVisitor : MethodInvocationVisitor
    {
        private readonly MigrationContext _context;
        private readonly Method _method;
        private readonly string _returnType;

        public ChangeMethodReturnTypeVisitor(MigrationContext context, Method method, string returnType)
            :base(context)
        {
            _context = context;
            _method = method;
            _returnType = returnType;
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            node = (LocalDeclarationStatementSyntax) base.VisitLocalDeclarationStatement(node);

            var initializer = node.Declaration
                .Descendants<EqualsValueClauseSyntax>()
                .FirstOrDefault();

            var invocation = initializer?
                .Descendants<InvocationExpressionSyntax>()
                .FirstOrDefault();

            if (invocation != null
                && InvocationMatches(invocation, _method.Type, _method.Name))
            {
                var newDeclaration = node.Declaration
                    .WithType(
                        SyntaxFactory.ParseTypeName(_returnType)
                            .WithTriviaFrom(node.Declaration.Type));

                node = node.ReplaceNode(node.Declaration, newDeclaration);

                _context.UpdateNodeType(newDeclaration, _returnType);
            }

            return node;
        }
    }
}
