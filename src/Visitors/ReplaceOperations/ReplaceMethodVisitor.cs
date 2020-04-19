using System.Collections.Generic;
using System.Linq;
using LibAdapter.Migrations;
using LibAdapter.Visitors.RenameOperations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Visitors.ReplaceOperations
{
    public class ReplaceMethodVisitor : ClassVisitor
    {
        private readonly Method _oldMethod;
        private readonly Method _newMethod;

        public ReplaceMethodVisitor(
            MigrationContext context,
            Method oldMethod,
            Method newMethod) : base(context)
        {
            _oldMethod = oldMethod;
            _newMethod = newMethod;
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            node = (ObjectCreationExpressionSyntax) base.VisitObjectCreationExpression(node);
            if (MethodMatches(node))
            {
                node = node.WithArgumentList(
                    CreateArgumentList(
                        _newMethod.Arguments,
                        node.ArgumentList.CloseParenToken.LeadingTrivia,
                        node.ArgumentList.CloseParenToken.TrailingTrivia));
            }

            return node;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            node = (InvocationExpressionSyntax) base.VisitInvocationExpression(node);
            if (MethodMatches(node))
            {
                node = (InvocationExpressionSyntax)new RenameMethodVisitor(
                        Context,
                        _oldMethod,
                        _newMethod.Name)
                    .VisitInvocationExpression(node);

                node = node.WithArgumentList(
                    CreateArgumentList(
                        _newMethod.Arguments,
                        node.ArgumentList.CloseParenToken.LeadingTrivia,
                        node.ArgumentList.CloseParenToken.TrailingTrivia));
            }

            return node;
        }

        private bool MethodMatches(ExpressionSyntax node)
        {
            var info = Context.GetMethodInfo(node);
            return info.TypeName == _oldMethod.Type
                   && info.MethodName == _oldMethod.Name
                   && ArgumentsMatch(info.Arguments, _oldMethod.Arguments);
        }

        private ArgumentListSyntax CreateArgumentList(IEnumerable<Argument> arguments, SyntaxTriviaList leadingTrivia, SyntaxTriviaList trailingTrivia)
        {
            var argList = ArgumentList(
                SeparatedList(arguments.Select(a =>
                {
                    var newIdentifier = IdentifierName(a.Type);
                    newIdentifier = (IdentifierNameSyntax) new AnnotationVisitor().Visit(newIdentifier);

                    Context.AddNewIdentifier(newIdentifier, new IdentifierInfo
                    {
                        TypeName = a.Type
                    });

                    if (!string.IsNullOrEmpty(a.DefaultValueExpression))
                    {
                        return Argument(ParseExpression(a.DefaultValueExpression));
                    }
                    else
                    {
                        return Argument(DefaultExpression(newIdentifier));
                    }
                })));

            argList = argList.WithCloseParenToken(argList.CloseParenToken
                .WithTrailingTrivia(trailingTrivia)
                .WithLeadingTrivia(leadingTrivia));
            return argList;
        }

        private bool ArgumentsMatch(string[] argumentTypes, Argument[] expectedArguments)
        {
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                if (argumentTypes[i] != expectedArguments[i].Type)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
