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
        private string FullTypeName { get; }

        private string OldMethodName { get; }

        private string NewMethodName { get; }

        private string[] OldArgumentTypes { get; }

        private string[] NewArgumentTypes { get; }

        public ReplaceMethodVisitor(
            MigrationContext context,
            string fullTypeName,
            string oldMethodName,
            string newMethodName,
            string[] oldArgumentTypes,
            string[] newArgumentTypes) : base(context)
        {
            OldArgumentTypes = oldArgumentTypes;
            NewArgumentTypes = newArgumentTypes;
            OldMethodName = oldMethodName;
            NewMethodName = newMethodName;
            FullTypeName = fullTypeName;
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            node = (ObjectCreationExpressionSyntax) base.VisitObjectCreationExpression(node);
            if (MethodMatches(node))
            {
                node = node.WithArgumentList(
                    CreateArgumentList(
                        NewArgumentTypes,
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
                        new Method { Type = FullTypeName, Name = OldMethodName},
                        NewMethodName)
                    .VisitInvocationExpression(node);

                node = node.WithArgumentList(
                    CreateArgumentList(
                        NewArgumentTypes,
                        node.ArgumentList.CloseParenToken.LeadingTrivia,
                        node.ArgumentList.CloseParenToken.TrailingTrivia));
            }

            return node;
        }

        private bool MethodMatches(ExpressionSyntax node)
        {
            var info = Context.GetMethodInfo(node);
            return info.TypeName == FullTypeName
                   && info.MethodName == OldMethodName
                   && ArgumentsMatch(info.Arguments, OldArgumentTypes);
        }

        private ArgumentListSyntax CreateArgumentList(string[] types, SyntaxTriviaList leadingTrivia, SyntaxTriviaList trailingTrivia)
        {
            var argList = ArgumentList(
                SeparatedList(types.Select(t =>
                {
                    var newIdentifier = IdentifierName(t);
                    newIdentifier = (IdentifierNameSyntax) new AnnotationVisitor().Visit(newIdentifier);

                    Context.AddNewIdentifier(newIdentifier, new IdentifierInfo
                    {
                        TypeName = t
                    });

                    return Argument(DefaultExpression(newIdentifier));
                })));

            argList = argList.WithCloseParenToken(argList.CloseParenToken
                .WithTrailingTrivia(trailingTrivia)
                .WithLeadingTrivia(leadingTrivia));
            return argList;
        }

        private bool ArgumentsMatch(string[] argumentTypes, string[] expectedTypes)
        {
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                if (argumentTypes[i] != expectedTypes[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
