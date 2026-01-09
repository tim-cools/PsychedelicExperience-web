using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Experiences
{
    public class WhenCreatingAnExperienceWithSurveyData : ApiTest
    {
        private ExperiencesResult _result;
        private TestAccount _account;
        private ShortGuid _experienceId;

        public WhenCreatingAnExperienceWithSurveyData(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);

            var data = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("data[substance]", "EiPLA"),
                new Tuple<string, string>("data[date]", "2020-01-08T14:25:00+01:00"),
                new Tuple<string, string>("data[title]", "title"),
                new Tuple<string, string>("data[description]", "experience"),
                new Tuple<string, string>("data[survey-id]", "2020-01-typm"),
                new Tuple<string, string>("data[intention-important]", "0"),
                new Tuple<string, string>("data[intention-why][]", "mental"),
                new Tuple<string, string>("data[intention-why][]", "spiritual"),
                new Tuple<string, string>("data[intention-called]", "1"),
                new Tuple<string, string>("data[intention-clear-wanted]", "1"),
                new Tuple<string, string>("data[preparation-important]", "0"),
                new Tuple<string, string>("data[preparation-did]", "0"),
                new Tuple<string, string>("data[preparation-why-no]", "dont-know"),
                new Tuple<string, string>("data[mindset-expectation]", "1"),
                new Tuple<string, string>("data[mindset-feelings-relaxed]", "1"),
                new Tuple<string, string>("data[mindset-feelings-nervous]", "2"),
                new Tuple<string, string>("data[mindset-feelings-fearful]", "3"),
                new Tuple<string, string>("data[mindset-feelings-excited]", "4"),
                new Tuple<string, string>("data[mindset-feelings-open]", "5"),
                new Tuple<string, string>("data[setting-context]", "medical"),
                new Tuple<string, string>("data[setting-where]", "home"),
                new Tuple<string, string>("data[setting-why]", "available"),
                new Tuple<string, string>("data[setting-environment-comfort]", "1"),
                new Tuple<string, string>("data[setting-environment-safety]", "5"),
                new Tuple<string, string>("data[setting-environment-available][]", "blindfolds"),
                new Tuple<string, string>("data[setting-environment-available][]", "friend"),
                new Tuple<string, string>("data[setting-facilitator]", "0"),
                new Tuple<string, string>("data[setting-facilitator-no]", "alone"),
                new Tuple<string, string>("data[mystical-unity]", "1"),
                new Tuple<string, string>("data[mystical-positive]", "1"),
                new Tuple<string, string>("data[mystical-time-space]", "3"),
                new Tuple<string, string>("data[mystical-beyond-words]", "5"),
                new Tuple<string, string>("data[mystical-challenging-thoughts]", "5"),
                new Tuple<string, string>("data[mystical-dying]", "1"),
                new Tuple<string, string>("data[mystical-never-end]", "2"),
                new Tuple<string, string>("data[integration-important]", "1"),
                new Tuple<string, string>("data[integration-integrated]", "1"),
                new Tuple<string, string>("data[integration-facilitator-services]", "yes-participated"),
                new Tuple<string, string>("data[integration-most-useful][]", "friendship"),
                new Tuple<string, string>("data[integration-most-useful][]", "friendship")
            };
            _experienceId = ApiClient.Experiences.Create(_account, null, data);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Experiences.GetList(_account);
        }

        [Fact]
        [Trait("category]", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            var experience = _result.Experiences.FirstOrDefault(element => element.ExperienceId == _experienceId);

            experience.ExperienceId.ShouldBe(_experienceId);
            experience.UserId.ShouldNotBe(ShortGuid.Empty);
            experience.Title.ShouldBe("title");
        }
    }
}