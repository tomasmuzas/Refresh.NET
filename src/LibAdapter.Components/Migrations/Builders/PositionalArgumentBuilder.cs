namespace LibAdapter.Components.Migrations.Builders
{
    public class PositionalArgumentBuilder
    {
        private readonly PositionalArgument _argument = new PositionalArgument();

        public PositionalArgumentBuilder OfType(string type)
        {
            _argument.Type = type;
            return this;
        }

        public PositionalArgumentBuilder WithDefaultValueExpression(string expression)
        {
            _argument.DefaultValueExpression = expression;
            return this;
        }

        public PositionalArgumentBuilder AtPosition(int position)
        {
            _argument.Position = position;
            return this;
        }

        public PositionalArgument Build()
        {
            return _argument;
        }
    }
}
