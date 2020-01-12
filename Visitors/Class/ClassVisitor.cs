using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.Class
{
    public class ClassVisitor : CSharpSyntaxRewriter
    {
        protected RefactoringContext Map { get; }

        public ClassVisitor(RefactoringContext map)
        {
            Map = map;
        }

        protected bool MatchesClassType(IdentifierNameSyntax identifier, string fullTypeName)
        {
            var identifierInfo = Map.GetIdentifierInfo(identifier);
            return identifierInfo?.TypeName == fullTypeName;
        }
    }
}
