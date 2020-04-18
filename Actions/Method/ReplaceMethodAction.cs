using LibAdapter.Visitors.Class;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions.Method
{
    public class ReplaceMethodAction : IAction
    {
        private string FullTypeName { get; }

        private string OldMethodName { get; }
        
        private string NewMethodName { get; }

        private string[] OldArgumentTypes { get; }

        private string[] NewArgumentTypes { get; }

        public ReplaceMethodAction(
            string fullTypeName, 
            string oldMethodName,
            string newMethodName,
            string[] oldArgumentTypes,
            string[] newArgumentTypes)
        {
            FullTypeName = fullTypeName;
            OldArgumentTypes = oldArgumentTypes;
            NewArgumentTypes = newArgumentTypes;
            OldMethodName = oldMethodName;
            NewMethodName = newMethodName;
        }

        public CSharpSyntaxRewriter ToVisitor(RefactoringContext map)
        {
            return new ReplaceMethodVisitor(map, FullTypeName, OldMethodName,NewMethodName, OldArgumentTypes, NewArgumentTypes);
        }
    }
}
