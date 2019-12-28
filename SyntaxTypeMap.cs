using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter
{
    public class SyntaxTypeMap
    {
        private readonly Dictionary<string, ISymbol> invocationMap =
            new Dictionary<string, ISymbol>();

        private readonly Dictionary<IdentifierNameSyntax, ISymbol> identifierMap =
            new Dictionary<IdentifierNameSyntax, ISymbol>();

        public List<SyntaxTree> Trees { get; private set; }

        public static SyntaxTypeMap FromProject(string filesPath, string mainDllPath)
        {
            var map = new SyntaxTypeMap();
            var references = Assembly.LoadFile(mainDllPath).GetReferencedAssemblies();

            var compilation = CSharpCompilation.Create("Code")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(mainDllPath));

            foreach (var name in references)
            {
                var assembly = Assembly.Load(name);
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(assembly.Location));
            }

            map.Trees = new List<SyntaxTree>();
            foreach (var filePath in Directory.GetFiles(filesPath, "*.cs", SearchOption.AllDirectories))
            {
                var code = File.ReadAllText(filePath);
                var tree = CSharpSyntaxTree.ParseText(code);
                map.Trees.Add(tree);
                compilation = compilation.AddSyntaxTrees(tree);
            }

            foreach (var tree in map.Trees)
            {
                var root = tree.GetCompilationUnitRoot();

                var semanticModel = compilation.GetSemanticModel(tree);
                var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocation in invocations)
                {
                    var containingSymbol = semanticModel.GetSymbolInfo(invocation).Symbol;
                    map.invocationMap.Add(invocation.ToFullString(), containingSymbol);
                }

                var identifiers = root.DescendantNodes().OfType<IdentifierNameSyntax>();
                foreach (var identifier in identifiers)
                {
                    var containingSymbol = semanticModel.GetSymbolInfo(identifier).Symbol;
                    //if (!map.identifierMap.ContainsKey(MakeKey(identifier)))
                    //{
                        map.identifierMap.Add(MakeKey(identifier), containingSymbol);
                    //}
                }
            }

            return map;
        }

        public static IdentifierNameSyntax MakeKey(IdentifierNameSyntax node)
        {
            return node;
        }

        public ISymbol GetInvocationSymbol(string invocationFullName)
        {
            return invocationMap[invocationFullName];
        }

        public ISymbol GetIdentifierSymbol(IdentifierNameSyntax identifier)
        {
            return identifierMap[MakeKey(identifier)];
        }

        public void UpdateInvocationMap(InvocationExpressionSyntax old, InvocationExpressionSyntax @new)
        {
            var value = invocationMap[old.ToFullString()];
            invocationMap.Remove(old.ToFullString());
            invocationMap.Add(@new.ToFullString(), value);
        }

        public void UpdateIdentifierMap(IdentifierNameSyntax old, IdentifierNameSyntax @new)
        {
            var key = MakeKey(old);
            var value = identifierMap[key];
            identifierMap.Remove(key);
            identifierMap.Add(MakeKey(@new), value);
        }
    }
}
