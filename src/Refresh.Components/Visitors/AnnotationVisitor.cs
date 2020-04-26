using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Components.Visitors
{
    public class AnnotationVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode Visit(SyntaxNode node)
        {
            node = base.Visit(node);
            node = node?.WithAdditionalAnnotations(GetAnnotation(node));

            return node;
        }

        private SyntaxAnnotation GetAnnotation(SyntaxNode node)
        {
            return new SyntaxAnnotation("TraceAnnotation", node.ToFullString() + node.GetHashCode());
        }
    }
}
