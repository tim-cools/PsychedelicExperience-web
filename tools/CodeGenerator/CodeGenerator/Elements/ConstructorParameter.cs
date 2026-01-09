namespace CodeGenerator.Elements
{
    internal class ConstructorParameter
    {
        public string Value { get; }
        public string Type { get; }
        public string Name { get; }

        public ConstructorParameter(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public ConstructorParameter(string value)
        {
            Value = value;
        }

        public string Decalration => Type + " " + Name.ToCamelCase();
    }
}