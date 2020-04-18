using System.Collections.Generic;
using System.Linq;
using LibAdapter.Visitors.MethodSignatureOperations;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations.MethodSignatureOperations
{
    public class AddArgumentsMigration : IMigration
    {
        private readonly Method _method;
        private readonly List<(Argument argument, int position)> _arguments;

        public AddArgumentsMigration(Method method, List<(Argument argument, int position)> arguments)
        {
            _method = method;
            _arguments = arguments;
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var visitor = new AddMethodParameterVisitor(
                context,
                _method.Type,
                _method.Name,
                _arguments.Select(a => (a.argument.DefaultValueExpression, a.position)));
            var ast = visitor.Visit(initialAST.GetRoot());
            return ast.SyntaxTree;
        }
    }
}
