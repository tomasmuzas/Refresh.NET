using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors.ChangeOperations;

namespace Refresh.Components.Migrations.ChangeOperations
{
    public class ChangeMemberTypeMigration : IMigration
    {
        private readonly string _type;
        private readonly string _memberName;
        private readonly string _returnType;

        public ChangeMemberTypeMigration(string type, string memberName, string returnType)
        {
            _type = type;
            _memberName = memberName;
            _returnType = returnType;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new ChangeMemberTypeVisitor(
                context,
                _type,
                _memberName,
                _returnType);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
