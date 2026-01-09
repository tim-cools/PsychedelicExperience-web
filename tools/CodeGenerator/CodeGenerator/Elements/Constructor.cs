using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Elements
{
    internal class Constructor : ICodeElement
    {
        private readonly List<Property> _properties;
        private readonly string _typeName;
        private readonly ConstructorParameter[] _constructorParameter;

        public Constructor(string typeName, params ConstructorParameter[] constructorParameter)
        {
            _typeName = typeName;
            _properties = new List<Property>();
            _constructorParameter = constructorParameter;
        }

        public Constructor(string typeName, List<Property> properties,
            params ConstructorParameter[] constructorParameter)
        {
            _typeName = typeName;
            _properties = properties;
            _constructorParameter = constructorParameter;
        }

        public void Build(CodeBuilder builder)
        {
            var parameters = Enumerable.Select(_properties, property => property.ParameterDecalration)
                .Union<string>(Enumerable.Where(_constructorParameter, parameter => parameter.Name != null)
                    .Select(parameter => parameter.Decalration))
                .ToArray();

            var baseParameters = Enumerable.Select(_constructorParameter, parameter => parameter.Value ?? parameter.Name)
                .ToArray<string>();

            var assignments = Enumerable.Select(_properties, property => property.Assignment)
                .ToArray<string>();

            builder
                .BeginMethod($"public {_typeName}({string.Join(", ", parameters)}) : base({string.Join(", ", baseParameters)})")
                .MethodContent(assignments)
                .EndMethod();
        }
    }
}