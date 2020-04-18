using Microsoft.CodeAnalysis;

namespace LibAdapter
{
    public interface IMigration
    {
        SyntaxTree Apply(SyntaxTree intialAST, MigrationContext context);
    }
}