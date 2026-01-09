using System;

namespace CodeGenerator.Blocks
{
    internal class Using : ICodeBlock
    {
        private readonly string _namespace;

        public Using(Line line)
        {
            _namespace = line.Get(1, nameof(Namespace));
        }

        public void Process(Line line)
        {
            throw new NotImplementedException();
        }

        public void Render(CodeBuilder builder)
        {
            builder.Root($"using {_namespace};");
        }
    }
}