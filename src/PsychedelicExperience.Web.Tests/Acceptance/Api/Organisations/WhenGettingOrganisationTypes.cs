using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenGettingOrganisationTypes : ApiTest
    {
        private Tag[] _types;

        public WhenGettingOrganisationTypes(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _types = ApiClient.Organisations.GetTypes();
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenNumberOfTypesShouldBeGreaterThanNull()
        {
            _types.Length.ShouldBeGreaterThan(0);
        }
    }
}