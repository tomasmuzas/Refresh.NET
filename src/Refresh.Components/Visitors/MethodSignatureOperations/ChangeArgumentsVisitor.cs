﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Refresh.Components.Visitors.MethodSignatureOperations
{
    public class ChangeArgumentsVisitor : MethodInvocationVisitor
    {
        private readonly Method _method;
        private readonly IEnumerable<PositionalArgument> _arguments;

        public ChangeArgumentsVisitor(
            MigrationContext context,
            Method method,
            IEnumerable<PositionalArgument> arguments) : base(context)
        {
            _method = method;
            _arguments = arguments;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax invocation)
        {
            invocation = (InvocationExpressionSyntax)base.VisitInvocationExpression(invocation);
            if (InvocationMatches(invocation, _method.Type, _method.Name))
            {
                var newInvocation = invocation;

                var argList = new List<ArgumentSyntax>();
                argList.AddRange(invocation.ArgumentList.Arguments);

                foreach (var arg in _arguments)
                {
                    argList[arg.Position - 1] = Argument(ParseExpression(arg.DefaultValueExpression)); 
                    // TODO: Add identifier type, if possible
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
