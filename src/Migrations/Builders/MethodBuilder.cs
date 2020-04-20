using System;
using System.Collections.Generic;

namespace LibAdapter.Migrations.Builders
{
    public class MethodBuilder
    {
        private readonly Method _method = new Method();

        public MethodBuilder()
        {
            _method.Arguments = new List<Argument>();
        }

        public MethodBuilder OfClass(string type)
        {
            _method.Type = type;
            return this;
        }

        public MethodBuilder WithName(string name)
        {
            _method.Name = name;
            return this;
        }

        public MethodBuilder WithArgument(Argument argument)
        {
            _method.Arguments.Add(argument);
            return this;
        }

        public MethodBuilder WithArgument(Action<ArgumentBuilder> argumentSetup)
        {
            var argumentBuilder = new ArgumentBuilder();
            argumentSetup.Invoke(argumentBuilder);
            var argument = argumentBuilder.Build();

            _method.Arguments.Add(argument);
            return this;
        }

        public Method Build()
        {
            return _method;
        }
    }
}
