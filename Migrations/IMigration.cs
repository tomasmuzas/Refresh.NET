using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations
{
    public interface IMigration
    {
        SyntaxTree Apply(SyntaxTree intialAST, MigrationContext context);
    }
}