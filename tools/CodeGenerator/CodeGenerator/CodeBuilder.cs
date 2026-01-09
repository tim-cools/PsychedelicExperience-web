using System.Collections.Generic;
using System.Text;

namespace CodeGenerator
{
    internal class CodeBuilder
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public CodeBuilder()
        {
        }

        public void BeginNamespace(string ns)
        {
            _stringBuilder.AppendLine($@"
namespace  {ns}
{{");
        }

        public void EndNamespace()
        {
            _stringBuilder.AppendLine("}");
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }

        public CodeBuilder BeginClass(string text)
        {
            _stringBuilder.AppendLine($"\t{text}");
            _stringBuilder.AppendLine("\t{");

            return this;
        }

        public CodeBuilder BeginMethod(string text)
        {
            _stringBuilder.AppendLine($"\r\n\t\t{text}");
            _stringBuilder.AppendLine("\t\t{");

            return this;
        }

        public CodeBuilder ClassElement(IEnumerable<ICodeElement> elements)
        {
            foreach (var elemnt in elements)
            {
                elemnt.Build(this);
            }

            return this;
        }

        public CodeBuilder ClassElement(ICodeElement element)
        {
            element.Build(this);

            return this;
        }

        public CodeBuilder ClassContent(string text)
        {
            _stringBuilder.AppendLine($"\t\t{text}");
            return this;
        }

        public CodeBuilder ClassContent(string[] texts)
        {
            foreach (var text in texts)
            {
                _stringBuilder.AppendLine($"\t\t{text}");
            }
            return this;
        }

        public CodeBuilder MethodContent(string text)
        {
            _stringBuilder.AppendLine($"\t\t\t{text}");
            return this;
        }

        public CodeBuilder MethodContent(string[] texts)
        {
            foreach (var text in texts)
            {
                _stringBuilder.AppendLine($"\t\t\t{text}");
            }
            return this;
        }

        public void EndClass()
        {
            _stringBuilder.AppendLine("\t}\r\n");
        }

        public void EndMethod()
        {
            _stringBuilder.AppendLine("\t\t}");
        }

        public void Root(string test)
        {
            _stringBuilder.AppendLine(test);
        }
    }
}