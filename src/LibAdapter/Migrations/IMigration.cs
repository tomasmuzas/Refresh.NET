using Microsoft.CodeAnalysis;

namespace LibAdapter.Migrations
{
    public interface IMigration
    {
        SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context);
    }
}