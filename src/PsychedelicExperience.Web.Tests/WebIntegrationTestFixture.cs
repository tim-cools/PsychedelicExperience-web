using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Web.Infrastructure;
using StructureMap;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PsychedelicExperience.Web.Tests
{
    public class WebIntegrationTestFixture : IntegrationTestFixture
    {
        protected override void InitializeContainer(ConfigurationExpression configuration)
        {
            configuration.AddRegistry<WebRegistry>();
        }
    }
}