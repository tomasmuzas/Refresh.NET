using System.Collections.Generic;
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
            SyntaxTypeMap map,
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

                newInvocation = newInvocation.WithArgumentList(
                    ArgumentList(
                        SeparatedList(argList)));

                invocation = invocation.CopyAnnotationsTo(newInvocation);
            }
            return invocation;
        }
    }
}
