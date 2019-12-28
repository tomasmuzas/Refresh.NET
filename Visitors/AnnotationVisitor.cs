using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors
{
    public class AnnotationVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode Visit(SyntaxNode node)
        {
            node = base.Visit(node);
            node = node?.WithAdditionalAnnotations(GetAnnotation(node));

            return node;
        }

        //public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        //{
        //    node = node.WithAdditionalAnnotations(GetAnnotation(node));
        //    return node;
        //}

        //public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        //{
        //    node = (IdentifierNameSyntax) base.VisitIdentifierName(node);
        //    node = node.WithAdditionalAnnotations(GetAnnotation(node));
        //    return node;
        //}

        private SyntaxAnnotation GetAnnotation(SyntaxNode node)
        {
            return new SyntaxAnnotation("TraceAnnotation", node.ToFullString() + node.Span.Start);
        }
    }
}
