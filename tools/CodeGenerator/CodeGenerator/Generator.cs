using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeGenerator.Blocks;

namespace CodeGenerator
{
    internal class Generator : IGenerator
    {
        private IDictionary<string, ConstructorInfo> _types;

        public string Extension => "gl";

        public Generator()
        {
            _types = typeof(Namespace).GetTypeInfo()
                .Assembly.GetTypes()
                .Where(type => typeof(ICodeBlock).IsAssignableFrom(type) && !type.GetTypeInfo().IsInterface)
                .ToDictionary(type => type.Name.ToCamelCase(), type => type.GetConstructor(new[] { typeof(Line) }));
        }

        public string Process(string[] content)
        {
            var builder = new CodeBuilder();
            var block = content
                .Aggregate<string, ICodeBlock>(null,
                    (current, line) => ParseLine(Line.Parse(line), current, builder));

            block?.Render(builder);

            builder.EndNamespace();

            return builder.ToString();
        }

        private ICodeBlock ParseLine(Line line, ICodeBlock block, CodeBuilder builder)
        {
            if (line.IsEmpty) return block;

            if (line.IsBlock)
            {
                block?.Render(builder);
                return CreateBlock(line);
            }

            if (block == null)
            {
                throw new InvalidOperationException("Block expected");
            }

            block.Process(line);

            return block;
        }

        private ICodeBlock CreateBlock(Line line)
        {
            var type = line.Get(0, nameof(Type));

            if (!_types.TryGetValue(type, out var constuctor))
            {
                throw new InvalidOperationException("Can't find type: " + type);
            }

            var codeBlock = constuctor.Invoke(new object[] {line});
            return codeBlock as ICodeBlock;
        }
    }
}