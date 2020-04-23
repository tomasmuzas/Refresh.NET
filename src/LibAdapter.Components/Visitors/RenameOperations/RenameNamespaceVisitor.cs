using LibAdapter.Components.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Components.Visitors.RenameOperations
{
    public class RenameNamespaceVisitor : CSharpSyntaxRewriter
    {
        private MigrationContext context { get; }

        private string OldNamespace { get; }
        
        private string NewNamespace { get; }

        public RenameNamespaceVisitor(MigrationContext context, string oldNamespace, string newNamespace)
        {
            context = context;
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
