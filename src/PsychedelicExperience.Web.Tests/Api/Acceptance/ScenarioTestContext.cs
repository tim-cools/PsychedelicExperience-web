using Marten;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Tests;
using StructureMap;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api
{
    public class ScenarioTestContext : ITestContext
    {
        public IContainer Container { get; }
        public IDocumentSession Session { get; }
        public IConfiguration Configuration { get; }

        public ScenarioTestContext(WebIntegrationTestFixture fixture)
        {
            Container = fixture.OpenContainerScope();
            Session = Container.GetInstance<IDocumentSession>();
            Configuration = Container.GetInstance<IConfiguration>();
        }
    }
}