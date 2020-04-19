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
            var methodIdentifier = Context.GetMethodIdentifier(invocation);
            var containingType = Context.GetNodeContainingClassType(invocation);
            return containingType == fullTypeName && methodIdentifier.ToString() == methodName;
        }
    }
}
