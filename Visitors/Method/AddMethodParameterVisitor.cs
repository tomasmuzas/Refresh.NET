using System.Collections.Generic;
using LibAdapter.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Visitors.Method
{
    public class AddMethodParameterVisitor : MethodInvocationVisitor
    {
        private string FullTypeName { get; }
        private string MethodName { get; }
        private IEnumerable<(string typeName, int position)> Arguments { get; }

        public AddMethodParameterVisitor(
            MigrationContext map,
            string fullTypeName,
            string MethodName,
            IEnumerable<(string, int)> arguments) : base(map)
        {
            FullTypeName = fullTypeName;
            this.MethodName = MethodName;
            Arguments = arguments;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax invocation)
        {
            invocation = (InvocationExpressionSyntax) base.VisitInvocationExpression(invocation);
            if (InvocationMatches(invocation, FullTypeName, MethodName))
            {
                InvocationExpressionSyntax newInvocation = invocation;

                var argList = new List<ArgumentSyntax>();
                argList.AddRange(invocation.ArgumentList.Arguments);

                foreach (var arg in Arguments)
                {
                    var argumentNode = Argument(ParseExpression(arg.typeName)); // TODO: Add identifier type, if possible
                    argList.Insert(arg.position - 1, argumentNode);
                }

                var argListSyntax = ArgumentList(SeparatedList(argList));
                argListSyntax = argListSyntax.WithCloseParenToken(argListSyntax.CloseParenToken
                    .WithTrailingTrivia(invocation.ArgumentList.CloseParenToken.TrailingTrivia)
                    .WithLeadingTrivia(invocation.ArgumentList.CloseParenToken.LeadingTrivia));

                newInvocation = newInvocation.WithArgumentList(argListSyntax);

                invocation = invocation.CopyAnnotationsTo(newInvocation);
            }
            return invocation;
        }
    }
}
