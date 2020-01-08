using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Visitors.Method
{
    public class ReorderMethodArgumentsVisitor: MethodInvocationVisitor
    {
        private string FullTypeName { get; }

        private string MethodName { get; }

        private int[] ArgumentOrder { get; }

        public ReorderMethodArgumentsVisitor(SyntaxTypeMap map, string fullTypeName, string methodName, int[] argumentOrder) : base(map)
        {
            FullTypeName = fullTypeName;
            MethodName = methodName;
            ArgumentOrder = argumentOrder;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            node = (InvocationExpressionSyntax)base.VisitInvocationExpression(node);
            if (InvocationMatches(node, FullTypeName, MethodName))
            {
                InvocationExpressionSyntax newInvocation = node;

                var argList = new List<(ArgumentSyntax syntax, int position)>();
                argList.AddRange(node.ArgumentList.Arguments.Select((s, i) => (s, ArgumentOrder[i])));
                argList.Sort((a, b) => a.Item2.CompareTo(b.position));

                newInvocation = newInvocation.WithArgumentList(
                    ArgumentList(
                        SeparatedList(argList.Select(arg => arg.syntax))));

                node = node.CopyAnnotationsTo(newInvocation);
            }

            return node;
        }
    }
}
