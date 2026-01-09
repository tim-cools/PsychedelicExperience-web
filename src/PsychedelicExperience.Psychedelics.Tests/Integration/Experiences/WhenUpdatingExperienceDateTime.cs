using System;
using System.Linq;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Experiences
{
    public class WhenUpdatingExperienceDateTime : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly DateTime _dateTime = DateTime.Now;
        private ExperienceId _experienceId;
        private Result _result;

        public WhenUpdatingExperienceDateTime(PsychedelicsIntegrationTestFixture fixture)
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
            var query = new UpdateExperienceDateTime(_experienceId, _userId, _dateTime);
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
                    .AreEqual(experience.DateTime, _dateTime)));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_experienceId).ToArray();
                events.Length.ShouldBe(2, () => JsonConvert.SerializeObject(events));

                Test.All(_ => _.IsNotNull(events.LastEventShouldBeOfType<ExperienceDateTimeChanged>(), (@event, __) => __
                    .AreEqual(@event.UserId, _userId, "userId")
                    .AreEqual(@event.ExperienceId, _experienceId)
                    .AreEqual(@event.DateTime, _dateTime)));
            });
        }
    }
}