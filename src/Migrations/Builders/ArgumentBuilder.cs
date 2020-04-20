namespace LibAdapter.Migrations.Builders
{
    public class ArgumentBuilder
    {
        private readonly Argument _argument = new Argument();

        public ArgumentBuilder WithType(string type)
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
