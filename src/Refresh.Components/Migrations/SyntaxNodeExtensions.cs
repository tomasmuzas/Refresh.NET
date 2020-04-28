using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Refresh.Components.Migrations
{
    public static class SyntaxNodeExtensions
    {
        public static IEnumerable<T> Descendants<T>(this SyntaxNode node)
        {
            return node.DescendantNodes().OfType<T>();
        }

        public static IEnumerable<T> DescendantsAndSelf<T>(this SyntaxNode node)
        {
            return node.DescendantNodesAndSelf().OfType<T>();
        }
    }
}
