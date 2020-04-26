using Microsoft.CodeAnalysis;

namespace Refresh.Components.Migrations
{
    public interface IMigration
    {
        SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context);
    }
}