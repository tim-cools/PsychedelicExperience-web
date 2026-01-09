using System.Collections.Generic;
using CodeGenerator.Elements;

namespace CodeGenerator.Blocks
{
    internal class CommandResult : ICodeBlock
    {
        private readonly string _name;
        private readonly string _baseType;
        private readonly List<Property> _properties = new List<Property>();

        public CommandResult(Line line)
        {
            _name = line.Get(1, "name");
            _baseType = line.GetOptional(2, "resultType") ?? "Result";
        }

        public void Process(Line line)
        {
            var property = new Property(line);
            _properties.Add(property);
        }

        public void Render(CodeBuilder builder)
        {
            builder.BeginClass($"public class {_name} : {_baseType}")
                .ClassElement(_properties)
                .ClassElement(new Constructor(_name))
                .ClassElement(new Constructor(
                    _name,
                    new ConstructorParameter("bool", "success"),
                    new ConstructorParameter("params ValidationError[]", "errors")))
                .ClassElement(new Constructor(
                    _name,
                    _properties,
                    new ConstructorParameter("true")
                ))
                .EndClass();
        }
    }
}