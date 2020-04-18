using System.Collections.Generic;
using LibAdapter.Visitors.Method;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions.Method
{
    public class AddMethodParametersAction : IAction
    {
        private string FullTypeName { get; }
        
        private string MethodName { get; }
        
        private IEnumerable<(string, int)> ArgumentTypes { get; }

        public AddMethodParametersAction(
            string fullTypeName,
            string methodName,
            IEnumerable<(string,int)> argumentTypes)
        {
            FullTypeName = fullTypeName;
            MethodName = methodName;
            ArgumentTypes = argumentTypes;
        }

        public CSharpSyntaxRewriter ToVisitor(MigrationContext map)
        {
            return new AddMethodParameterVisitor(map, FullTypeName, MethodName, ArgumentTypes);
        }
    }
}
