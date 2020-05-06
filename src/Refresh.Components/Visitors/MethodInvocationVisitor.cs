using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;

namespace Refresh.Components.Visitors
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
            var methodIdentifier = GetMethodIdentifier(invocation, methodName);
            if (methodIdentifier == null)
            {
                return false;
            }

            var containingType = Context.GetNodeContainingClassType(methodIdentifier);
            return containingType == fullTypeName;
        }

        public IdentifierNameSyntax GetMethodIdentifier(ExpressionSyntax invocation)
        {
            var nodes = invocation
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>().ToList();
            
            return nodes.ElementAtOrDefault(1) ?? nodes.ElementAtOrDefault(0) ?? null;
        }

        public IdentifierNameSyntax GetMethodIdentifier(ExpressionSyntax invocation, string methodName)
        {
            return invocation
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .FirstOrDefault(i => i.WithoutTrivia().ToString() == methodName);
        }
    }
}
