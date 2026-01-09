namespace CodeGenerator
{
    internal interface ICodeBlock
    {
        void Process(Line line);
        void Render(CodeBuilder builder);
    }
}