using LibAdapter.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Visitors.Method
{
    public class RenameMethodVisitor : MethodInvocationVisitor
    {
        private string FullTypeName { get; }

        private string OldMethodName { get; }

        private string NewMethodName { get; }

        public RenameMethodVisitor(
            MigrationContext map, 
            string fullTypeName, 
            string oldMethodName,
            string newMethodName) : base(map)
        {
            FullTypeName = fullTypeName;
            OldMethodName = oldMethodName;
            NewMethodName = newMethodName;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax invocation)
        {
            if (InvocationMatches(invocation, FullTypeName, OldMethodName))
            {
                var oldIdentifier = Map.GetMethodIdentifier(invocation);
                var oldIdentifierInfo = Map.GetIdentifierInfo(oldIdentifier);

                var newIdentifier = IdentifierName(NewMethodName);
                newIdentifier = oldIdentifier.CopyAnnotationsTo(newIdentifier);

                Map.UpdateIdentifierInfo(newIdentifier, new IdentifierInfo {TypeName = oldIdentifierInfo.TypeName});

                var newInvocation = invocation.ReplaceNode(
                    oldIdentifier,
                    newIdentifier);

                invocation = invocation.CopyAnnotationsTo(newInvocation);
                
                var invocationInfo = Map.GetMethodInfo(invocation);
                Map.UpdateInvocationInfo(invocation, new MethodInfo
                {
                    TypeName = invocationInfo.TypeName,
                    MethodName = NewMethodName
                });
            }
            return invocation;
        }
    }
}
