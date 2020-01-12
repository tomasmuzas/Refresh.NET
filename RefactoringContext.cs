using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter
{
    public class RefactoringContext
    {
        private readonly Dictionary<string, MethodInfo> methodMap =
            new Dictionary<string, MethodInfo>();

        private readonly Dictionary<string, IdentifierInfo> identifierMap =
            new Dictionary<string, IdentifierInfo>();

        private SyntaxTree Tree { get; }

        public CompilationUnitSyntax Root { get; set; }

        public RefactoringContext(SyntaxTree tree)
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
                var containingType = semanticModel.GetTypeInfo(type).Type;
                var constructorInfo = new MethodInfo
                {
                    TypeName = containingType.ToString(),
                    MethodName = "ctor"
                };

                var args = new List<string>();
                foreach (var argument in type.ArgumentList?.Arguments ?? Enumerable.Empty<ArgumentSyntax>())
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
                var symInfo = semanticModel.GetSymbolInfo(invocation);
                var containingSymbol = symInfo.CandidateSymbols.Any() ? 
                    symInfo.CandidateSymbols.First().ContainingSymbol
                    : semanticModel.GetSymbolInfo(invocation).Symbol.ContainingSymbol;

                var methodInfo = new MethodInfo
                {
                    TypeName = containingSymbol.ToString(),
                    MethodName = GetMethodIdentifier(invocation)?.Identifier.ValueText
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
                if(containingSymbol != null){
                    identifierMap.Add(MakeKey(identifier), new IdentifierInfo
                    {
                        TypeName = containingSymbol.ToString()
                    });
                }
            }
        }

        public IdentifierNameSyntax GetMethodIdentifier(InvocationExpressionSyntax invocation)
        {
            var nodes = invocation.Expression
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>().ToList();
            return nodes.LastOrDefault() ?? null;
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
            identifierMap.TryGetValue(MakeKey(identifier), out var value);
            return value;
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
