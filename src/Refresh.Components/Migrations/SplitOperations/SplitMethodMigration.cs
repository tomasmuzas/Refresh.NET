using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors.SplitOperations;

namespace Refresh.Components.Migrations.SplitOperations
{
    public class SplitMethodMigration : IMigration
    {
        private readonly Method _method;
        private readonly Method _newMethod1;
        private readonly Method _newMethod2;

        public SplitMethodMigration(Method method, Method newMethod1, Method newMethod2)
        {
            _method = method;
            _newMethod1 = newMethod1;
            _newMethod2 = newMethod2;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new SplitMethodVisitor(context, _method, _newMethod1, _newMethod2);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
