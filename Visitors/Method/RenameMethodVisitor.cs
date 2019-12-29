﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace LibAdapter.Visitors.Method
{
    public class RenameMethodVisitor : MethodInvocationVisitor
    {
        private string FullTypeName { get; }

        private string OldMethodName { get; }

        public string NewMethodName { get; }

        public RenameMethodVisitor(
            SyntaxTypeMap map, 
            string fullTypeName, 
            string oldMethodName,
            string newMethodName) : base(map)
        {
            FullTypeName = fullTypeName;
            OldMethodName = oldMethodName;
            NewMethodName = newMethodName;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax invocation)
        {
            if (InvocationMatches(invocation, FullTypeName, OldMethodName))
            {
                var oldIdentifier = Map.GetMethodIdentifier(invocation);
                var oldIdentifierInfo = Map.GetIdentifierInfo(oldIdentifier);

                var newIdentifier = IdentifierName(NewMethodName);
                newIdentifier = oldIdentifier.CopyAnnotationsTo(newIdentifier);

                Map.UpdateIdentifierInfo(newIdentifier, new IdentifierTypeInfo {TypeName = oldIdentifierInfo.TypeName});

                var newInvocation = invocation.ReplaceNode(
                    oldIdentifier,
                    newIdentifier);

                invocation = invocation.CopyAnnotationsTo(newInvocation);
                
                var invocationInfo = Map.GetInvocationInfo(invocation);
                Map.UpdateInvocationInfo(invocation, new MethodTypeInfo
                {
                    TypeName = invocationInfo.TypeName,
                    MethodName = NewMethodName
                });
            }
            return invocation;
        }
    }
}
