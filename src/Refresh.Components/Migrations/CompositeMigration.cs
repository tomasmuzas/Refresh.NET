using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Refresh.Components.Migrations
{
    public class CompositeMigration : IMigration
    {
        private readonly List<IMigration> _migrations;

        public CompositeMigration()
        {
            _migrations = new List<IMigration>();
        }

        public CompositeMigration(params IMigration[] migrations)
        {
            _migrations = new List<IMigration>(migrations);
        }

        public void AddMigration(IMigration migration)
        {
            _migrations.Add(migration);
        }

        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            foreach (var migration in _migrations)
            {
                initialAST = migration.Apply(initialAST, context);
            }

            return initialAST;
        }
    }
}
