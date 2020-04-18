using LibAdapter.Visitors.Class;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations
{
    public class RenameClassMigration : IMigration
    {
        private readonly string _type;
        private readonly string _newName;

        private RenameClassVisitor _visitor;

        public RenameClassMigration(string type, string newName)
        {
            _type = type;
            _newName = newName;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            _visitor = new RenameClassVisitor(context, _type, _newName);
            var newAst = _visitor.Visit(initialAST.GetRoot());

            return newAst.SyntaxTree;
        }
    }
}
