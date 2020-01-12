using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.Method
{
    public class MethodInvocationVisitor: CSharpSyntaxRewriter
    {
        protected RefactoringContext Map { get;}

        public MethodInvocationVisitor(RefactoringContext map)
        {
            Map = map;
        }

        protected bool InvocationMatches(InvocationExpressionSyntax invocation, string fullTypeName, string methodName)
        {
            var methodInfo = Map.GetMethodInfo(invocation);
            return methodInfo.TypeName == fullTypeName && methodInfo.MethodName == methodName;
        }
    }
}
