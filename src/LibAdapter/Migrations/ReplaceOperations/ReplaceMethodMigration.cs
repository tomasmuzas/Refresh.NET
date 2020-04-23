using LibAdapter.Visitors.ReplaceOperations;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations.ReplaceOperations
{
    public class ReplaceMethodMigration : IMigration
    {
        private readonly Method _oldMethod;
        private readonly Method _newMethod;

        public ReplaceMethodMigration(Method oldMethod, Method newMethod)
        {
            _oldMethod = oldMethod;
            _newMethod = newMethod;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new ReplaceMethodVisitor(
                context,
                _oldMethod,
                _newMethod);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
