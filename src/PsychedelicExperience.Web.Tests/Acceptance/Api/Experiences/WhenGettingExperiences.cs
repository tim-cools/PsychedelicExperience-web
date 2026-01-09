using System;
using System.Linq;
using System.Threading;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Experiences
{
    public class WhenGettingExperiences : ApiTest
    {
        private ExperiencesResult _result;
        private TestAccount _account;
        private ShortGuid _experienceId;

        public WhenGettingExperiences(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _experienceId = ApiClient.Experiences.Create(_account, "title123");
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Experiences.GetList(_account);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            var experience = _result.Experiences.FirstOrDefault(element => element.ExperienceId == _experienceId);

            experience.ExperienceId.ShouldBe(_experienceId);
            experience.UserId.ShouldNotBe(ShortGuid.Empty);
        }
    }
}