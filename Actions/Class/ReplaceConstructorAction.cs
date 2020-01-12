using LibAdapter.Visitors.Class;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions.Class
{
    public class ReplaceConstructorAction : IAction
    {
        private string FullTypeName { get; }

        private string[] OldArgumentTypes { get; }

        private string[] NewArgumentTypes { get; }

        public ReplaceConstructorAction(string fullTypeName, string[] oldArgumentTypes, string[] newArgumentTypes)
        {
            FullTypeName = fullTypeName;
            OldArgumentTypes = oldArgumentTypes;
            NewArgumentTypes = newArgumentTypes;
        }

        public CSharpSyntaxRewriter ToVisitor(RefactoringContext map)
        {
            return new ReplaceMethodVisitor(map, FullTypeName, "ctor", null, OldArgumentTypes, NewArgumentTypes);
        }
    }
}
