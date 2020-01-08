using System;
using System.Collections.Generic;
using System.Text;
using LibAdapter.Visitors.Class;
using LibAdapter.Visitors.Method;
using Microsoft.CodeAnalysis.CSharp;

namespace LibAdapter.Actions.Method
{
    public class ReorderMethodParametersAction: IAction
    {
        private string FullTypeName { get; }

        private string MethodName { get; }

        private int[] ArgumentOrder { get; }

        public ReorderMethodParametersAction(string fullTypeName, string methodName, int[] argumentOrder)
        {
            FullTypeName = fullTypeName;
            MethodName = methodName;
            ArgumentOrder = argumentOrder;
        }

        public CSharpSyntaxRewriter ToVisitor(SyntaxTypeMap map)
        {
            return new ReorderMethodArgumentsVisitor(map, FullTypeName, MethodName, ArgumentOrder);
        }
    }
}
