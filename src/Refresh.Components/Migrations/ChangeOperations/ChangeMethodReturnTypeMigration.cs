using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors.ChangeOperations;

namespace Refresh.Components.Migrations.ChangeOperations
{
    public class ChangeMethodReturnTypeMigration : IMigration
    {
        private readonly Method _method;
        private readonly FullType _returnType;

        public ChangeMethodReturnTypeMigration(Method method, FullType returnType)
        {
            _method = method;
            _returnType = returnType;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new ChangeMethodReturnTypeVisitor(context, _method, _returnType);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
