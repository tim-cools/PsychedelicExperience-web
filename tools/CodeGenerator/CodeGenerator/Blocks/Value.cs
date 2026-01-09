using System.Collections.Generic;
using System.Linq;
using CodeGenerator.Elements;

namespace CodeGenerator.Blocks
{
    internal class Value : ICodeBlock
    {
        private readonly string _name;
        private readonly List<Property> _properties = new List<Property>();

        public Value(Line line)
        {
            _name = line.Get(1, "name");
        }

        public void Process(Line line)
        {
            var property = new Property(line, true);
            _properties.Add(property);
        }

        public void Render(CodeBuilder builder)
        {
            builder
                .BeginClass($"public class {_name}")
                .ClassElement(_properties)
                .ClassElement(new Constructor(_name))
                .ClassElement(new Constructor(_name, _properties.Where(property => property.Name != "Id").ToList()))
                .EndClass();
        }
    }
}