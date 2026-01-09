using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Substances.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Substances
{
    public class WhenGettingSubstances : ApiTest
    {
        private TestAccount _account;
        private SubstanceResult _result;

        public WhenGettingSubstances(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Substances.Get(_account);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.Substances.Length.ShouldBeGreaterThan(0);
        }
    }
}