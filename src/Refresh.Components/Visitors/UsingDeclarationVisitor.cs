using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;

namespace Refresh.Components.Visitors
{
    public class UsingDeclarationVisitor : CSharpSyntaxRewriter
    {
        private readonly MigrationContext _context;

        public UsingDeclarationVisitor(MigrationContext context)
        {
            _context = context;
        }

        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            node = (BlockSyntax) base.VisitBlock(node);

            var usingStatements = node
                .Descendants<UsingStatementSyntax>()
                .ToList();

            foreach (var usingStatement in usingStatements)
            {
                var position = node.Statements.IndexOf(usingStatement);
                var newStatements =  node.Statements.RemoveAt(position);

                var declarationStatement = SyntaxFactory.LocalDeclarationStatement(
                    new SyntaxToken(),
                    SyntaxFactory.Token(SyntaxKind.UsingKeyword).WithTrailingTrivia(SyntaxFactory.Space),
                    new SyntaxTokenList(),
                    usingStatement.Declaration,
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    .WithTriviaFrom(usingStatement);

                newStatements = newStatements.Insert(position, declarationStatement);

                var currentPosition = position + 1;
                foreach (var statement in usingStatement.Statement.Descendants<StatementSyntax>())
                {
                    newStatements = newStatements.Insert(currentPosition, statement);
                    currentPosition++;
                }

                node = node.WithStatements(newStatements);
            }

            return node;
        }
    }
}
