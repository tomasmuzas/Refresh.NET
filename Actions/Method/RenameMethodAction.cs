using LibAdapter.Visitors.Method;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions.Method
{
    public class RenameMethodAction : IAction
    {
        private string FullTypeName { get; }

        private string OldName { get; }

        private string NewName { get; }

        public RenameMethodAction(string fullTypeName, string oldName, string newName)
        {
            FullTypeName = fullTypeName;
            OldName = oldName;
            NewName = newName;
        }

        public CSharpSyntaxRewriter ToVisitor(SyntaxTypeMap map)
        {
            return new RenameMethodVisitor(map, FullTypeName,OldName, NewName);
        }
    }
}
