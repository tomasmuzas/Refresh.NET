using LibAdapter.Migrations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors
{
    public class ClassVisitor : CSharpSyntaxRewriter
    {
        protected MigrationContext Context { get; }

        public ClassVisitor(MigrationContext context)
        {
            Context = context;
        }

        protected bool MatchesClassType(IdentifierNameSyntax identifier, string fullTypeName)
        {
            var identifierInfo = Context.GetNodeContainingClassType(identifier);
            return identifierInfo == fullTypeName;
        }
    }
}
