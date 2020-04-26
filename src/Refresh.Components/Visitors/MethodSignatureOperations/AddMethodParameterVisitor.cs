using System.Collections.Generic;
using LibAdapter.Components.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Components.Visitors.MethodSignatureOperations
{
    public class AddMethodParameterVisitor : MethodInvocationVisitor
    {
        private string FullTypeName { get; }
        private string MethodName { get; }
        private IEnumerable<PositionalArgument> Arguments { get; }

        public AddMethodParameterVisitor(
            MigrationContext context,
            string fullTypeName,
            string MethodName,
            IEnumerable<PositionalArgument> arguments) : base(context)
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
                    var argument = Argument(!string.IsNullOrEmpty(arg.DefaultValueExpression) ?
                        ParseExpression(arg.DefaultValueExpression):
                        DefaultExpression(IdentifierName(arg.Type)));

                    argList.Insert(arg.Position - 1, argument);
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
