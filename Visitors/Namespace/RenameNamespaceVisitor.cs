using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.Namespace
{
    public class RenameNamespaceVisitor : CSharpSyntaxRewriter
    {
        protected SyntaxTypeMap Map { get; }

        private string OldNamespace { get; }
        
        private string NewNamespace { get; }

        public RenameNamespaceVisitor(SyntaxTypeMap map, string oldNamespace, string newNamespace)
        {
            Map = map;
            OldNamespace = oldNamespace;
            NewNamespace = newNamespace;
        }

        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            node = (UsingDirectiveSyntax) base.VisitUsingDirective(node);
            if (node.Name.ToString() == OldNamespace)
            {
                var newIdentifier = SyntaxFactory.IdentifierName(NewNamespace);
                newIdentifier = node.Name.CopyAnnotationsTo(newIdentifier);
                node = node.WithName(newIdentifier);
            }
            return node;
        }
    }
}
