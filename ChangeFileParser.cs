using System.Collections.Generic;
using System.IO;
using LibAdapter.Actions;
using LibAdapter.Actions.Class;
using LibAdapter.Actions.Method;
using LibAdapter.Actions.Namespace;

namespace LibAdapter
{
    public class ChangeFileParser
    {
        public static IEnumerable<IAction> ParseFile(string filePath)
        {
            var actions = new List<IAction>();
            foreach (var line in File.ReadLines(filePath))
            {
                var action = line.Split(" ")[0];
                switch (action)
                {
                    case "rename":
                    {
                        var arguments = line.Split(" ");
                        ParseRename(arguments, actions);
                        break;
                    }
                    case "add":
                    {
                        var arguments = line.Split(" ");
                        actions.Add(new AddMethodParametersAction(arguments[2], arguments[3], arguments[4].Split(",")));
                        break;
                    }
                }
            }

            return actions;
        }

        private static void ParseRename(string[] arguments, List<IAction> actions)
        {
            switch (arguments[1])
            {
                case "method":
                {
                    actions.Add(new RenameMethodAction(arguments[2], arguments[3], arguments[4]));
                    break;
                }
                case "type":
                {
                    actions.Add(new RenameClassAction(arguments[2], arguments[3]));
                    break;
                }
                case "namespace":
                {
                    actions.Add(new RenameNamespaceAction(arguments[2], arguments[3]));
                    break;
                }
            }
        }
    }
}
