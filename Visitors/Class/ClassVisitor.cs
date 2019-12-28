﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibAdapter.Visitors.Class
{
    public class ClassVisitor : CSharpSyntaxRewriter
    {
        protected SyntaxTypeMap Map { get; }

        public ClassVisitor(SyntaxTypeMap map)
        {
            Map = map;
        }

        protected bool MatchesClassType(IdentifierNameSyntax identifier, string fullTypeName)
        {
            var containingClass = Map.GetIdentifierSymbol(identifier);
            return containingClass.ToString() == fullTypeName && containingClass is INamedTypeSymbol;
        }
    }
}
