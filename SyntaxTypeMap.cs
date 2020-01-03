using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter
{
    public class SyntaxTypeMap
    {
        private readonly Dictionary<string, MethodInfo> methodMap =
            new Dictionary<string, MethodInfo>();

        private readonly Dictionary<string, IdentifierInfo> identifierMap =
            new Dictionary<string, IdentifierInfo>();

        private SyntaxTree Tree { get; }

        public CompilationUnitSyntax Root { get; set; }

        public SyntaxTypeMap(SyntaxTree tree)
        {
            Tree = tree;
            Root = tree.GetCompilationUnitRoot();
        }

        public void PopulateFromCompilation(CSharpCompilation compilation)
        {
            var semanticModel = compilation.GetSemanticModel(Tree);

            var types = Root.DescendantNodesAndSelf().OfType<ObjectCreationExpressionSyntax>().ToList();
            foreach (var type in types)
            {
                var containingType = semanticModel.GetSymbolInfo(type).Symbol.ContainingType;
                var constructorInfo = new MethodInfo
                {
                    TypeName = containingType.ToString(),
                    MethodName = "ctor"
                };

                var args = new List<string>();
                foreach (var argument in type.ArgumentList.Arguments)
                {
                    var symbolInfo = semanticModel.GetTypeInfo(argument.Expression).ConvertedType;
                    args.Add(symbolInfo.ToString());
                }

                constructorInfo.Arguments = args.ToArray();
                methodMap.Add(MakeKey(type), constructorInfo);
            }

            var invocations = Root.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().ToList();
            foreach (var invocation in invocations)
            {
                var containingSymbol = semanticModel.GetSymbolInfo(invocation).Symbol.ContainingSymbol;
                var methodInfo = new MethodInfo
                {
                    TypeName = containingSymbol.ToString(),
                    MethodName = GetMethodIdentifier(invocation).Identifier.ValueText
                };

                var args = new List<string>();
                foreach (var argument in invocation.ArgumentList.Arguments)
                {
                    var symbolInfo = semanticModel.GetTypeInfo(argument.Expression).ConvertedType;
                    args.Add(symbolInfo.ToString());
                }

                methodInfo.Arguments = args.ToArray();
                methodMap.Add(MakeKey(invocation), methodInfo);
            }

            var identifiers = Root.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().ToList();

            foreach (var identifier in identifiers)
            {
                var containingSymbol = semanticModel.GetSymbolInfo(identifier).Symbol;
                identifierMap.Add(MakeKey(identifier), new IdentifierInfo
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

        public void UpdateInvocationInfo(InvocationExpressionSyntax invocation, MethodInfo info)
        {
            methodMap.Remove(MakeKey(invocation));
            methodMap.Add(MakeKey(invocation), info);
        }

        public MethodInfo GetMethodInfo(ExpressionSyntax method)
        {
            return methodMap[MakeKey(method)];
        }

        public IdentifierInfo GetIdentifierInfo(IdentifierNameSyntax identifier)
        {
            return identifierMap[MakeKey(identifier)];
        }

        public void AddNewIdentifier(IdentifierNameSyntax identifier, IdentifierInfo info)
        {
            identifierMap.Add(MakeKey(identifier), info);
        }

        public void UpdateIdentifierInfo(IdentifierNameSyntax identifier, IdentifierInfo info)
        {
            identifierMap.Remove(MakeKey(identifier));
            identifierMap.Add(MakeKey(identifier), info);
        }
    }
}
