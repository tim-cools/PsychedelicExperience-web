using StructureMap;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Tests;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PsychedelicExperience.Psychedelics.Tests
{
    public class PsychedelicsIntegrationTestFixture : IntegrationTestFixture
    {
        protected override void InitializeContainer(ConfigurationExpression configuration)
        {
            configuration.ConfigureDummyIdentity();
            configuration.AddRegistry<MembershipRegistry>();
            configuration.AddRegistry<PsychedelicsRegistry>();
        }
    }
}