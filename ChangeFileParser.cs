using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibAdapter.Actions;
using LibAdapter.Actions.Class;
using LibAdapter.Actions.Method;
using LibAdapter.Actions.Namespace;
using LibAdapter.Visitors.Class;

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
                    case "add_argument":
                    {
                        var match = Regex.Match(line,
                            @"add_argument method:((?:[a-z]*\.)*[a-z]*)\.([a-z0-9_]*) ((?:(?:default_value|expression)\:\'?([^\']+)\'? \d,?)*)",
                            RegexOptions.IgnoreCase);

                        var args = match.Groups;

                        actions.Add(new AddMethodParametersAction(
                            args[1].Value, 
                            args[2].Value, 
                            args[3].Value
                                .Split(",")
                                .Select(arg =>
                                {
                                    var groups = Regex.Match(arg, @"'?([^']*)'? (\d)", RegexOptions.IgnoreCase)
                                        .Groups;

                                    return (groups[1].Value, int.Parse(groups[2].Value));
                                })));
                        break;
                    }
                    case "reorder_arguments":
                    {
                        var match = Regex.Match(line,
                            @"reorder_arguments method:((?:[a-z]*\.)*[a-z]*)\.([a-z0-9_]*) ((?:(?:\d+),?)*)",
                            RegexOptions.IgnoreCase);

                        var args = match.Groups;

                        actions.Add(new ReorderMethodParametersAction(
                            args[1].Value,
                            args[2].Value,
                            args[3].Value
                                .Split(",")
                                .Select(int.Parse)
                                .ToArray()));
                        break;
                    }
                    case "replace_constructor":
                    {
                        var arguments = line.Split(" ");
                        actions.Add(new ReplaceConstructorAction(
                            arguments[1], 
                            arguments[2].Split(","), 
                            arguments[3].Split(",")));
                        break;
                    }
                    case "replace_method":
                    {
                        var match = Regex.Match(line,
                            @"replace_method ([a-z\.]*) ([a-z]*)\(([a-z,\.]*)\) ([a-z]*)\(([a-z,\.]*)\)",
                            RegexOptions.IgnoreCase);

                        var args = match.Groups;
                        actions.Add(new ReplaceMethodAction(
                            args[1].Value,
                            args[2].Value,
                            args[4].Value,
                            args[3].Value.Split(","),
                            args[5].Value.Split(",")));
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
