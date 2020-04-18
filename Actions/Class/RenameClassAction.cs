using LibAdapter.Visitors.Class;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions.Class
{
    public class RenameClassAction : IAction
    {
        private string FullTypeName { get; }

        private string NewName { get; }

        public RenameClassAction(string fullTypeName, string newName)
        {
            FullTypeName = fullTypeName;
            NewName = newName;
        }

        public CSharpSyntaxRewriter ToVisitor(MigrationContext map)
        {
            return new RenameTypeVisitor(map, FullTypeName, NewName);
        }
    }
}
