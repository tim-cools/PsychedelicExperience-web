using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Experiences
{
    public class WhenUpdatingExperienceTitle : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private ExperienceId _experienceId;
        private Result _result;

        public WhenUpdatingExperienceTitle(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _experienceId = context.AddExperience(_userId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new UpdateExperienceTitle(_experienceId, _userId, new Title("title"));
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheExperienceLevelShouldBeStored()
        {
            SessionScope(context =>
            {
                Test.All(_ => _.IsNotNull(context.Session.Load<Experience>(_experienceId), (experience, __) => __
                    .AreEqual(experience.Title, new Title("title"))));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_experienceId);
                events.Count.ShouldBe(2, () => JsonConvert.SerializeObject(events));

                Test.All(_ => _.IsNotNull(events.LastEventShouldBeOfType<ExperienceTitleChanged>(), (@event, __) => __
                    .AreEqual(@event.UserId, _userId, "userId")
                    .AreEqual(@event.ExperienceId, _experienceId)
                    .AreEqual(@event.Title, new Title("title"))));
            });
        }
    }
}