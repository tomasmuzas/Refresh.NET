using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.Method
{
    public class MethodInvocationVisitor: CSharpSyntaxRewriter
    {
        protected SyntaxTypeMap Map { get;}

        public MethodInvocationVisitor(SyntaxTypeMap map)
        {
            Map = map;
        }

        protected bool InvocationMatches(InvocationExpressionSyntax invocation, string fullTypeName, string methodName)
        {
            var methodIdentifier = GetMethodIdentifier(invocation);
            var methodIdentifierName = methodIdentifier.Identifier.ValueText;
            var containingClass = Map.GetInvocationSymbol(invocation.ToFullString()).ContainingSymbol;
            return containingClass.ToString() == fullTypeName && methodIdentifierName == methodName;
        }

        protected IdentifierNameSyntax GetMethodIdentifier(InvocationExpressionSyntax invocation)
        {
            return invocation.Expression
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .ElementAt(1);
        }
    }
}
