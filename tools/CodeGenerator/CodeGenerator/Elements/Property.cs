namespace CodeGenerator
{
    internal class Property : ICodeElement
    {
        private readonly bool _addDefaultValues;
        public string Type { get; }
        public string Name { get; }

        public Property(Line line, bool addDefaultValues = false)
        {
            _addDefaultValues = addDefaultValues;
            Type = line.Get(0, "type");
            Name = line.Get(1, "name");
        }

        public void Build(CodeBuilder builder)
        {
            if (Type.StartsWith("IList<") && _addDefaultValues)
            {
                var implementationType = Type.Substring(1);
                builder.ClassContent($"public {Type} {Name} {{ get; }} = new {implementationType}();");
            }
            else
            {
                builder.ClassContent($"public {Type} {Name} {{ get; set; }}");
            }
        }

        public string ParameterName => Name.ToCamelCase();
        public string ParameterDecalration => $"{Type} {ParameterName}";
        public string Assignment => $"{Name} = {ParameterName};";
    }
}