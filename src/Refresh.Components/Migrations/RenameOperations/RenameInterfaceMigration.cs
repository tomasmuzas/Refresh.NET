using System.Linq;
using LibAdapter.Components.Visitors.RenameOperations;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Components.Migrations.RenameOperations
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

            var parts = _type.Split(".");
            var ns = string.Join(".", parts.Take(parts.Length - 1));
            var newType = ns + "." + _newName;

            context.ReplaceType(_type, newType);

            return ast.SyntaxTree;
        }
    }
}
