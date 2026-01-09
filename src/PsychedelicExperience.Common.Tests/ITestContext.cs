using Marten;
using Microsoft.Extensions.Configuration;
using StructureMap;

namespace PsychedelicExperience.Common.Tests
{
    public interface ITestContext
    {
        IContainer Container { get; }
        IDocumentSession Session { get; }
        IConfiguration Configuration { get; }
    }
}