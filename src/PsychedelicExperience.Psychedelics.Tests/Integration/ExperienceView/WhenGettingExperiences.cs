using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.ExperienceView
{
    public class WhenGettingExperiences : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly List<ExperienceId> _ids = new List<ExperienceId>();

        private ExperiencesResult _result;

        public WhenGettingExperiences(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddAdministrator(_userId);

            _ids.Add(AddExperience(context, "What else 1"));
            _ids.Add(AddExperience(context, "What else 2"));
            _ids.Add(AddExperience(context, "What else 3"));
        }

        private ExperienceId AddExperience(TestContext<IMediator> context, string whatElse)
        {
            return context.AddExperience(_userId, whatElse);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetExperiences(_userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenExperiencesShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Experiences.Length.ShouldBe(3);
            _result.Experiences.Select(result => result.ExperienceId)
                .ShouldBeSubsetOf(_ids.Select(value => new ShortGuid(value.Value)));
        }
    }
}
