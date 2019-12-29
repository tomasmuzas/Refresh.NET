using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter
{
    public class SyntaxTypeMap
    {
        private readonly Dictionary<string, MethodTypeInfo> invocationMap =
            new Dictionary<string, MethodTypeInfo>();

        private readonly Dictionary<string, IdentifierTypeInfo> identifierMap =
            new Dictionary<string, IdentifierTypeInfo>();

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
                var containingSymbol = semanticModel.GetSymbolInfo(invocation).Symbol.ContainingSymbol;
                var methodInfo = new MethodTypeInfo
                {
                    TypeName = containingSymbol.ToString(),
                    MethodName = GetMethodIdentifier(invocation).Identifier.ValueText
                };
                invocationMap.Add(MakeKey(invocation), methodInfo);
            }

            var identifiers = Root.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().ToList();

            foreach (var identifier in identifiers)
            {
                var containingSymbol = semanticModel.GetSymbolInfo(identifier).Symbol;
                identifierMap.Add(MakeKey(identifier), new IdentifierTypeInfo
                {
                    TypeName = containingSymbol.ToString()
                });
            }
        }

        public IdentifierNameSyntax GetMethodIdentifier(InvocationExpressionSyntax invocation)
        {
            return invocation.Expression
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .ElementAt(1);
        }

        private static string MakeKey(SyntaxNode node)
        {
            return node.GetAnnotations("TraceAnnotation").First().Data;
        }

        public void UpdateInvocationInfo(InvocationExpressionSyntax invocation, MethodTypeInfo info)
        {
            invocationMap.Remove(MakeKey(invocation));
            invocationMap.Add(MakeKey(invocation), info);
        }

        public MethodTypeInfo GetInvocationInfo(InvocationExpressionSyntax invocation)
        {
            return invocationMap[MakeKey(invocation)];
        }

        public IdentifierTypeInfo GetIdentifierInfo(IdentifierNameSyntax identifier)
        {
            return identifierMap[MakeKey(identifier)];
        }

        public void AddNewIdentifier(IdentifierNameSyntax identifier, IdentifierTypeInfo info)
        {
            identifierMap.Add(MakeKey(identifier), info);
        }

        public void UpdateIdentifierInfo(IdentifierNameSyntax identifier, IdentifierTypeInfo info)
        {
            identifierMap.Remove(MakeKey(identifier));
            identifierMap.Add(MakeKey(identifier), info);
        }
    }
}
