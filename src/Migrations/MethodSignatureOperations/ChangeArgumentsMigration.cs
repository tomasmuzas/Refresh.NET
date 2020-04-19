using System.Collections.Generic;
using LibAdapter.Visitors.MethodSignatureOperations;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations.MethodSignatureOperations
{
    public class ChangeArgumentsMigration : IMigration
    {
        private readonly Method _method;
        private readonly List<(Argument, int)> _arguments;

        public ChangeArgumentsMigration(Method method, List<(Argument, int)> arguments)
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
