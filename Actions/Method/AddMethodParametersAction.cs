using System.Collections.Generic;
using LibAdapter.Visitors.Method;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions.Method
{
    public class AddMethodParametersAction : IAction
    {
        private string FullTypeName { get; }
        
        private string MethodName { get; }
        
        private IEnumerable<string> ArgumentTypes { get; }

        public AddMethodParametersAction(
            string fullTypeName,
            string methodName,
            IEnumerable<string> argumentTypes)
        {
            FullTypeName = fullTypeName;
            MethodName = methodName;
            ArgumentTypes = argumentTypes;
        }

        public CSharpSyntaxRewriter ToVisitor(SyntaxTypeMap map)
        {
            return new AddMethodParameterVisitor(map, FullTypeName, MethodName, ArgumentTypes);
        }
    }
}
