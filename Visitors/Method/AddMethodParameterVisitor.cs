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
        private IEnumerable<string> ArgumentTypes { get; }

        public AddMethodParameterVisitor(
            SyntaxTypeMap map,
            string fullTypeName,
            string MethodName,
            IEnumerable<string> argumentTypes
            ) : base(map)
        {
            FullTypeName = fullTypeName;
            this.MethodName = MethodName;
            ArgumentTypes = argumentTypes;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax invocation)
        {
            if (InvocationMatches(invocation, FullTypeName, MethodName))
            {
                InvocationExpressionSyntax newInvocation = invocation;
                foreach (var argType in ArgumentTypes)
                {
                    var newIdentifier = IdentifierName(argType);
                    newIdentifier = (IdentifierNameSyntax) new AnnotationVisitor().Visit(newIdentifier);
                        
                    newInvocation = newInvocation.WithArgumentList(newInvocation.ArgumentList.AddArguments(
                        Argument(DefaultExpression(newIdentifier))
                            .WithLeadingTrivia(Space)));

                    Map.AddNewIdentifier(newIdentifier, new IdentifierTypeInfo {TypeName = argType});
                }

                invocation = invocation.CopyAnnotationsTo(newInvocation);
            }
            return invocation;
        }
    }
}
