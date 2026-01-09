using System.Collections.Generic;
using CodeGenerator.Elements;

namespace CodeGenerator.Blocks
{
    internal class Command : ICodeBlock
    {
        private readonly string _name;
        private readonly string _resultType;
        private readonly List<Property> _properties = new List<Property>();

        public Command(Line line)
        {
            _name = line.Get(1, "name");
            _resultType = line.GetOptional(2, "resultType") ?? "Result";
        }

        public void Process(Line line)
        {
            var property = new Property(line);
            _properties.Add(property);
        }

        public void Render(CodeBuilder builder)
        {
            builder
                .BeginClass($"public class {_name} : IRequest<{_resultType}>")
                .ClassElement(_properties)
                .ClassElement(new Constructor(_name, _properties))
                .EndClass();
        }
    }
}