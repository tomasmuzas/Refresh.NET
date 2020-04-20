using System.Collections.Generic;
using System.Linq;
using LibAdapter.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Visitors.ReplaceOperations
{
    public class ReplaceClassVisitor : CSharpSyntaxRewriter
    {
        private readonly MigrationContext _context;
        private readonly string _type; 
        private readonly string _newType;
        private readonly IEnumerable<Argument> _constructorArguments;

        public ReplaceClassVisitor(
            MigrationContext context,
            string type,
            string newType,
            IEnumerable<Argument> constructorArguments)
        {
            _context = context;
            _type = type;
            _newType = newType;
            _constructorArguments = constructorArguments;
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            node = (ObjectCreationExpressionSyntax)base.VisitObjectCreationExpression(node);
            if (_context.GetNodeContainingClassType(node) == _type)
            {
                node = node.WithArgumentList(
                    CreateArgumentList(
                        _constructorArguments,
                        node.ArgumentList.CloseParenToken.LeadingTrivia,
                        node.ArgumentList.CloseParenToken.TrailingTrivia));
            }

            return node;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            node = (IdentifierNameSyntax)base.VisitIdentifierName(node);

            if (_context.GetNodeType(node) == _type)
            {
                node = node.WithIdentifier(SyntaxFactory.Identifier(_newType)
                    .WithTrailingTrivia(node.Identifier.TrailingTrivia)
                    .WithLeadingTrivia(node.Identifier.LeadingTrivia));

                node = node.CopyAnnotationsTo(node);
            }

            return node;
        }

        private ArgumentListSyntax CreateArgumentList(IEnumerable<Argument> arguments, SyntaxTriviaList leadingTrivia, SyntaxTriviaList trailingTrivia)
        {
            var argList = ArgumentList(
                SeparatedList(arguments.Select(a =>
                {
                    var newIdentifier = IdentifierName(a.Type);
                    newIdentifier = (IdentifierNameSyntax)new AnnotationVisitor().Visit(newIdentifier);

                    _context.UpdateNodeType(newIdentifier, a.Type);

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
    }
}
