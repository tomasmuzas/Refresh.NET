using System;
using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations
{
    public class RenameClassMigration : IMigration
    {
        private readonly string _type;
        private readonly string _newName;

        public RenameClassMigration(string type, string newName)
        {
            _type = type;
            _newName = newName;
        }

        public SyntaxTree Apply(SyntaxTree intialAST, MigrationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
