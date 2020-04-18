using LibAdapter.Visitors;
using LibAdapter.Visitors.Namespace;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions.Namespace
{
    public class RenameNamespaceAction : IAction
    {
        private string OldNamespace { get; }

        private string NewNamespace { get; }

        public RenameNamespaceAction(string oldNamespace, string newNamespace)
        {
            OldNamespace = oldNamespace;
            NewNamespace = newNamespace;
        }

        public CSharpSyntaxRewriter ToVisitor(MigrationContext map)
        {
            return new RenameNamespaceVisitor(map, OldNamespace, NewNamespace);
        }
    }
}
