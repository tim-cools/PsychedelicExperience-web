using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Experiences
{
    public class WhenGettingAnExperience : ApiTest
    {
        private ExperienceDetails _result;
        private ShortGuid _experienceId;
        private TestAccount _account;

        public WhenGettingAnExperience(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            _experienceId = ApiClient.Experiences.Create(_account, "title123");
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Experiences.Get(_account, _experienceId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.ExperienceId.ShouldBe(_experienceId);
            _result.UserId.ShouldNotBe(ShortGuid.Empty);
        }
    }
}