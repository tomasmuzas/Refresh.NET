using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Migrations
{
    public class MigrationContext
    {
        private readonly Dictionary<string, string> containingNodeTypes = new Dictionary<string, string>();
        
        private readonly Dictionary<string, string> nodeTypes = new Dictionary<string, string>();

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

        public void UpdateNodeType(SyntaxNode node, string newType)
        {
            nodeTypes.Remove(MakeKey(node));
            nodeTypes.Add(MakeKey(node), newType);
        }

        public void ReplaceType(string type, string newType)
        {
            var nodesToReplace = nodeTypes
                .Where(kv => kv.Value == type)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var node in nodesToReplace)
            {
                nodeTypes.Remove(node);
            }

            foreach (var node in nodesToReplace)
            {
                nodeTypes.Add(node, newType);
            }

            var containingNodesToReplace = containingNodeTypes
                .Where(kv => kv.Value == type)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var node in containingNodesToReplace)
            {
                containingNodeTypes.Remove(node);
            }

            foreach (var node in containingNodesToReplace)
            {
                containingNodeTypes.Add(node, newType);
            }
        }

        private static string MakeKey(SyntaxNode node)
        {
            return node.GetAnnotations("TraceAnnotation").First().Data;
        }
    }
}
