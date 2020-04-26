using LibAdapter.Components.Visitors.RenameOperations;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Components.Migrations.RenameOperations
{
    public class RenameMemberMigration : IMigration
    {
        private readonly string _type;
        private readonly string _memberName;
        private readonly string _newMemberName;

        public RenameMemberMigration(string type, string memberName, string newMemberName)
        {
            _type = type;
            _memberName = memberName;
            _newMemberName = newMemberName;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new RenameMemberVisitor(context, _type, _memberName, _newMemberName);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
