﻿using System.Collections.Generic;
using System.Linq;
using LibAdapter.Visitors.MethodSignatureOperations;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations.MethodSignatureOperations
{
    public class AddArgumentsMigration : IMigration
    {
        private readonly Method _method;
        private readonly List<PositionalArgument> _arguments;

        public AddArgumentsMigration(Method method, List<PositionalArgument> arguments)
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
                _arguments);
            var ast = visitor.Visit(initialAST.GetRoot());
            return ast.SyntaxTree;
        }
    }
}
