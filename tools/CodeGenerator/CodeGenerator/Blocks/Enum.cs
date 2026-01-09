using System.Collections.Generic;
using CodeGenerator.Elements;

namespace CodeGenerator.Blocks
{
    internal class Enum : ICodeBlock
    {
        private readonly string _name;
        private readonly List<EnumValue> _values = new List<EnumValue>();

        public Enum(Line line)
        {
            _name = line.Get(1, "name");
        }

        public void Process(Line line)
        {
            var type = line.Get(0, "name");
            var name = line.GetOptional(1, "value");
            var property = new EnumValue(type, name);
            _values.Add(property);
        }

        public void Render(CodeBuilder builder)
        {
            builder
                .BeginClass($"public enum {_name}")
                .ClassElement(_values)
                .EndClass();
        }
    }
}