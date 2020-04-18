using LibAdapter.Migrations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.Method
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
            var methodInfo = Context.GetMethodInfo(invocation);
            return methodInfo.TypeName == fullTypeName && methodInfo.MethodName == methodName;
        }
    }
}
