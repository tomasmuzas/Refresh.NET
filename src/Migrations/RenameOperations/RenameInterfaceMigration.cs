using LibAdapter.Visitors.Class;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations.RenameOperations
{
    public class RenameInterfaceMigration : IMigration
    {
        private readonly string _type;
        private readonly string _newName;

        public RenameInterfaceMigration(string type, string newName)
        {
            _type = type;
            _newName = newName;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new RenameClassVisitor(context, _type, _newName);
            var ast = visitor.Visit(initialAST.GetRoot());
            return ast.SyntaxTree;
        }
    }
}
