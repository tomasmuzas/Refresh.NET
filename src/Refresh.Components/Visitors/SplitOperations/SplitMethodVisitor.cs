using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;
using Refresh.Components.Visitors.ReplaceOperations;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Refresh.Components.Visitors.SplitOperations
{
    public class SplitMethodVisitor : MethodInvocationVisitor
    {
        private readonly MigrationContext _context;
        private readonly Method _method;
        private readonly Method _newMethod1;
        private readonly Method _newMethod2;

        public SplitMethodVisitor(
            MigrationContext context,
            Method method,
            Method newMethod1,
            Method newMethod2) : base(context)
        {
            _context = context;
            _method = method;
            _newMethod1 = newMethod1;
            _newMethod2 = newMethod2;
        }

        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            node = (BlockSyntax) base.VisitBlock(node);

            var matchingInvocations = node
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(i => InvocationMatches(i, _method.Type, _method.Name))
                .ToList();

            foreach (var invocation in matchingInvocations)
            {
                var visitor = new ReplaceMethodVisitor(_context, _method, _newMethod1);
                node = node.ReplaceNode(
                    invocation,
                    visitor.VisitInvocationExpression(invocation)
                        .WithTriviaFrom(invocation));

                var variable = invocation.Expression
                    .DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .First();

                node = node.AddStatements(ExpressionStatement(
                    CreateInvocation(variable.ToString(), _newMethod2.Name, _newMethod2.Arguments))
                    .WithTriviaFrom(node.Statements.Last()));
            }

            return node;
        }

        private InvocationExpressionSyntax CreateInvocation(
            string variableName,
            string methodName,
            IList<Argument> arguments)
        {
            var argList = 
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
                }));

            return InvocationExpression(
                ParseExpression($"{variableName}.{methodName}"),
                ArgumentList(argList)
            );
        }
    }
}
