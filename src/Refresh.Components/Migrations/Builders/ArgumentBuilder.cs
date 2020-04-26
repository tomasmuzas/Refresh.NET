namespace Refresh.Components.Migrations.Builders
{
    public class ArgumentBuilder
    {
        private readonly Argument _argument = new Argument();

        public ArgumentBuilder OfType(string type)
        {
            _argument.Type = type;
            return this;
        }

        public ArgumentBuilder WithDefaultValueExpression(string expression)
        {
            _argument.DefaultValueExpression = expression;
            return this;
        }

        public Argument Build()
        {
            return _argument;
        }
    }
}
