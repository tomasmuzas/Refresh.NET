using System.Collections.Generic;
using LibAdapter.Components.Visitors.MethodSignatureOperations;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Components.Migrations.MethodSignatureOperations
{
    public class RemoveArgumentsMigration : IMigration
    {
        private readonly Method _method;
        private readonly IEnumerable<int> _positions;

        public RemoveArgumentsMigration(Method method, IEnumerable<int> positions)
        {
            _method = method;
            _positions = positions;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new RemoveArgumentsVisitor(context, _method, _positions);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
