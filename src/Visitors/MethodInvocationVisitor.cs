using System.Linq;
using LibAdapter.Migrations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors
{
    public class MethodInvocationVisitor: CSharpSyntaxRewriter
    {
        protected MigrationContext Context { get;}

        public MethodInvocationVisitor(MigrationContext context)
        {
            Context = context;
        }

        protected bool InvocationMatches(InvocationExpressionSyntax invocation, string fullTypeName, string methodName)
        {
            var methodIdentifier = GetMethodIdentifier(invocation);
            var containingType = Context.GetNodeContainingClassType(invocation);
            return containingType == fullTypeName && methodIdentifier.ToString() == methodName;
        }

        public IdentifierNameSyntax GetMethodIdentifier(ExpressionSyntax invocation)
        {
            var nodes = invocation
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>().ToList();
            return nodes.ElementAtOrDefault(1) ?? nodes.ElementAtOrDefault(0) ?? null; 
        }
    }
}
