namespace CodeGenerator
{
    internal interface IGenerator
    {
        string Extension { get; }
        string Process(string[] content);
    }
}