using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Refresh.Components.Migrations;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Refresh.Tool.SampleMigrations
{
    public class CSharp7to8 : IMigration
    {
        public SyntaxTree Apply(SyntaxTree initialAST, MigrationContext context)
        {
            var switchVisitor = new SwitchSimplificationVisitor();

            initialAST = switchVisitor.Visit(initialAST.GetRoot()).SyntaxTree;

            var usingVisitor = new UsingDeclarationVisitor();

            initialAST = usingVisitor.Visit(initialAST.GetRoot()).SyntaxTree;

            return initialAST;
        }

        public class UsingDeclarationVisitor : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                node = (BlockSyntax)base.VisitBlock(node);

                var usingStatements = node
                    .Descendants<UsingStatementSyntax>()
                    .ToList();

                foreach (var usingStatement in usingStatements)
                {
                    var position = node.Statements.IndexOf(usingStatement);
                    var newStatements = node.Statements.RemoveAt(position);

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

        public class SwitchSimplificationVisitor : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                node = (BlockSyntax)base.VisitBlock(node);

                var switchStatements = node
                    .Descendants<SwitchStatementSyntax>();

                foreach (var switchStatement in switchStatements)
                {
                    var governingExpression = switchStatement.Expression
                        .WithTrailingTrivia(Space);

                    var arms = switchStatement.Sections
                        .Select(ConstructSwitchArm)
                        .ToList();

                    var armsList = SeparatedList(
                        arms,
                        Enumerable.Repeat(arms, arms.Count - 1)
                            .Select(a => Token(
                                SyntaxTriviaList.Empty,
                                SyntaxKind.CommaToken,
                                SyntaxTriviaList.Create(EndOfLine(Environment.NewLine)))));

                    var switchExpression = SwitchExpression(
                        governingExpression,
                        Token(SyntaxKind.SwitchKeyword).WithTrailingTrivia(EndOfLine(Environment.NewLine)),
                        switchStatement.OpenBraceToken,
                        armsList,
                        switchStatement.CloseBraceToken.WithTrailingTrivia(SyntaxTriviaList.Empty))
                        .WithLeadingTrivia(switchStatement.GetLeadingTrivia());

                    var position = node.Statements.IndexOf(switchStatement);

                    node = node.WithStatements(
                        node.Statements
                            .Remove(switchStatement)
                            .Insert(
                                position,
                                ExpressionStatement(switchExpression)
                                    .WithTrailingTrivia(EndOfLine(Environment.NewLine))));
                }

                return node;
            }

            private SwitchExpressionArmSyntax ConstructSwitchArm(SwitchSectionSyntax section)
            {
                var isDefault = section
                    .Descendants<DefaultSwitchLabelSyntax>()
                    .Any();

                var returnStatement = section
                    .Descendants<ReturnStatementSyntax>()
                    .FirstOrDefault();

                if (returnStatement?.Expression == null)
                {
                    return null;
                }

                PatternSyntax pattern;
                if (!isDefault)
                {
                    var labelExpression = section.Labels
                        .FirstOrDefault()
                        ?.DescendantsAndSelf<ExpressionSyntax>()
                        .FirstOrDefault();

                    pattern = ConstantPattern(labelExpression);
                }
                else
                {
                    pattern = DiscardPattern();
                }

                var arm = SwitchExpressionArm(
                    pattern,
                    null,
                    Token(SyntaxKind.EqualsGreaterThanToken)
                        .WithLeadingTrivia(Space)
                        .WithTrailingTrivia(Space),
                    returnStatement.Expression)
                    .WithLeadingTrivia(section.GetLeadingTrivia());

                if (isDefault)
                {
                    arm = arm.WithTrailingTrivia(EndOfLine(Environment.NewLine));
                }

                return arm;
            }
        }
    }
}
