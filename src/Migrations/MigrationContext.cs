using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Migrations
{
    public class MigrationContext
    {
        private readonly Dictionary<string, string> containingNodeTypes = new Dictionary<string, string>();
        
        private readonly Dictionary<string, string> nodeTypes = new Dictionary<string, string>();

        private readonly Dictionary<string, IdentifierInfo> identifierMap =
            new Dictionary<string, IdentifierInfo>();

        public void Populate(CSharpCompilation compilation, SyntaxTree tree)
        {
            var semanticModel = compilation.GetSemanticModel(tree);

            foreach (var node in tree.GetCompilationUnitRoot().DescendantNodesAndSelf())
            {
                var symbolInfo = semanticModel.GetSymbolInfo(node);

                nodeTypes.Add(MakeKey(node), symbolInfo.Symbol?.ToString());

                containingNodeTypes.Add(MakeKey(node),
                    symbolInfo.Symbol?.ContainingType?.ToString() ??
                    symbolInfo.Symbol?.ToString());
            }
        }

        private static string MakeKey(SyntaxNode node)
        {
            return node.GetAnnotations("TraceAnnotation").First().Data;
        }

        public string GetNodeContainingClassType(SyntaxNode node)
        {
            return containingNodeTypes[MakeKey(node)];
        }

        public string GetNodeType(SyntaxNode node)
        {
            return nodeTypes[MakeKey(node)];
        }

        public void UpdateNodeContainingClassType(SyntaxNode node, string newContainingType)
        {
            containingNodeTypes.Remove(MakeKey(node));
            containingNodeTypes.Add(MakeKey(node), newContainingType);
        }

        public void AddNewIdentifier(IdentifierNameSyntax identifier, IdentifierInfo info)
        {
            identifierMap.Add(MakeKey(identifier), info);
        }
    }
}
