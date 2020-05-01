using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors.ChangeOperations;

namespace Refresh.Components.Migrations.ChangeOperations
{
    public class ChangeMemberTypeMigration : IMigration
    {
        private readonly FullType _type;
        private readonly string _memberName;
        private readonly FullType _returnType;

        public ChangeMemberTypeMigration(FullType type, string memberName, FullType returnType)
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
