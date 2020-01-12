using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions
{
    public interface IAction
    {
        CSharpSyntaxRewriter ToVisitor(RefactoringContext map);
    }
}
