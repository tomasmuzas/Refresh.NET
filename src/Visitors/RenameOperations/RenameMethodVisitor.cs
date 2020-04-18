using LibAdapter.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Visitors.RenameOperations
{
    public class RenameMethodVisitor : MethodInvocationVisitor
    {
        private string FullTypeName { get; }

        private string OldMethodName { get; }

        private string NewMethodName { get; }

        public RenameMethodVisitor(
            MigrationContext context, 
            Migrations.Method method,
            string newMethodName) : base(context)
        {
            FullTypeName = method.Type;
            OldMethodName = method.Name;
            NewMethodName = newMethodName;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax invocation)
        {
            if (InvocationMatches(invocation, FullTypeName, OldMethodName))
            {
                var oldIdentifier = Context.GetMethodIdentifier(invocation);
                var oldIdentifierInfo = Context.GetIdentifierInfo(oldIdentifier);

                var newIdentifier = IdentifierName(NewMethodName);
                newIdentifier = oldIdentifier.CopyAnnotationsTo(newIdentifier);

                Context.UpdateIdentifierInfo(newIdentifier, new IdentifierInfo {TypeName = oldIdentifierInfo.TypeName});

                var newInvocation = invocation.ReplaceNode(
                    oldIdentifier,
                    newIdentifier);

                invocation = invocation.CopyAnnotationsTo(newInvocation);
                
                var invocationInfo = Context.GetMethodInfo(invocation);
                Context.UpdateInvocationInfo(invocation, new MethodInfo
                {
                    TypeName = invocationInfo.TypeName,
                    MethodName = NewMethodName
                });
            }
            return invocation;
        }
    }
}
