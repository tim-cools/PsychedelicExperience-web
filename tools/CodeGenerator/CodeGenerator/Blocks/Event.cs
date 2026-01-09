using System.Collections.Generic;

namespace CodeGenerator.Blocks
{
    internal class Event : ICodeBlock
    {
        private readonly string _name;
        private readonly List<Property> _properties = new List<Property>();

        public Event(Line line)
        {
            _name = line.Get(1, "name");
        }

        public void Process(Line line)
        {
            var property = new Property(line);
            _properties.Add(property);
        }

        public void Render(CodeBuilder builder)
        {
            builder
                .BeginClass($"public class {_name} : Event")
                .ClassElement(_properties)
                .EndClass();
        }
    }
}