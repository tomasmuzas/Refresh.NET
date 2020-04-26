using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Refresh.Components.Visitors.MethodSignatureOperations;

namespace Refresh.Components.Migrations.MethodSignatureOperations
{
    public class ChangeArgumentsMigration : IMigration
    {
        private readonly Method _method;
        private readonly List<PositionalArgument> _arguments;

        public ChangeArgumentsMigration(Method method, List<PositionalArgument> arguments)
        {
            _method = method;
            _arguments = arguments;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new ChangeArgumentsVisitor(context, _method, _arguments);
            var ast = visitor.Visit(initialAST.GetRoot());

            return ast.SyntaxTree;
        }
    }
}
