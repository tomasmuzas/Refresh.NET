using System.Collections.Generic;
using System.Linq;
using LibAdapter.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter
{
    public class SyntaxTypeMap
    {
        private readonly Dictionary<string, ISymbol> invocationMap =
            new Dictionary<string, ISymbol>();

        private readonly Dictionary<string, ISymbol> identifierMap =
            new Dictionary<string, ISymbol>();

        public SyntaxTree Tree { get; private set; }

        public CompilationUnitSyntax Root { get; set; }

        public SyntaxTypeMap(SyntaxTree tree)
        {
            Tree = tree;
            Root = tree.GetCompilationUnitRoot();
        }

        public void PopulateFromCompilation(CSharpCompilation compilation)
        {
            var semanticModel = compilation.GetSemanticModel(Tree);
            var invocations = Root.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().ToList();
            foreach (var invocation in invocations)
            {
                var containingSymbol = semanticModel.GetSymbolInfo(invocation).Symbol;
                invocationMap.Add(MakeKey(invocation), containingSymbol);
            }

            var identifiers = Root.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().ToList();

            foreach (var identifier in identifiers)
            {
                var containingSymbol = semanticModel.GetSymbolInfo(identifier).Symbol;
                identifierMap.Add(MakeKey(identifier), containingSymbol);
            }
        }

        private static string MakeKey(SyntaxNode node)
        {
            return node.GetAnnotations("TraceAnnotation").First().Data;
        }

        public ISymbol GetInvocationSymbol(InvocationExpressionSyntax invocation)
        {
            return invocationMap[MakeKey(invocation)];
        }

        public ISymbol GetIdentifierSymbol(IdentifierNameSyntax identifier)
        {
            return identifierMap[MakeKey(identifier)];
        }
    }
}
