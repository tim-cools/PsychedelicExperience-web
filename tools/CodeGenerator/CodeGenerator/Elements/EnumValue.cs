namespace CodeGenerator.Elements
{
    internal class EnumValue : ICodeElement
    {
        private readonly string _name;
        private readonly string _value;

        public EnumValue(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public void Build(CodeBuilder builder)
        {
            var expression = _value != null ? $"{_name} = {_value}," : $"{_name},";

            builder.ClassContent(expression);
        }
    }
}