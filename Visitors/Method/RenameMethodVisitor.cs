using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Visitors.Method
{
    public class RenameMethodVisitor : MethodInvocationVisitor
    {
        private string FullTypeName { get; }

        private string OldMethodName { get; }

        public string NewMethodName { get; }

        public RenameMethodVisitor(
            SyntaxTypeMap map, 
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
                var newInvocation = invocation.ReplaceNode(
                    GetMethodIdentifier(invocation)
                        .WithAdditionalAnnotations(new SyntaxAnnotation("TraceAnnotation", null)),
                    IdentifierName(NewMethodName));
                //Map.UpdateInvocationMap(invocation, newInvocation);

                invocation = invocation.CopyAnnotationsTo(newInvocation);
            }
            return invocation;
        }
    }
}
