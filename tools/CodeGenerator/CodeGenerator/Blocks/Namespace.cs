using System;

namespace CodeGenerator.Blocks
{
    internal class Namespace : ICodeBlock
    {
        private readonly string _namespace;

        public Namespace(Line line)
        {
            _namespace = line.Get(1, nameof(_namespace));
        }

        public void Process(Line line)
        {
            throw new NotImplementedException();
        }

        public void Render(CodeBuilder builder)
        {
            builder.BeginNamespace(_namespace);
        }
    }
}