using LibAdapter.Visitors.Class;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations.RenameOperations
{
    public class RenameClassMigration : IMigration
    {
        private readonly string _type;
        private readonly string _newName;

        public RenameClassMigration(string type, string newName)
        {
            _type = type;
            _newName = newName;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new RenameClassVisitor(context, _type, _newName);
            var newAst = visitor.Visit(initialAST.GetRoot());

            return newAst.SyntaxTree;
        }
    }
}
