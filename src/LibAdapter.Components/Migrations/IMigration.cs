using Microsoft.CodeAnalysis;

namespace LibAdapter.Components.Migrations
{
    public interface IMigration
    {
        SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context);
    }
}