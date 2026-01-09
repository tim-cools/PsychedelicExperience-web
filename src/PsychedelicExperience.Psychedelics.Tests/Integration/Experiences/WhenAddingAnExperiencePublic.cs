using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using Shouldly;
using Xunit;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PrivacyLevel = PsychedelicExperience.Psychedelics.Experiences.PrivacyLevel;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Experiences
{
    public class WhenAddingAnExperiencePublic : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly ExperienceId _experienceId = ExperienceId.New();
        private Result _result;

        public WhenAddingAnExperiencePublic(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            var data = new ExperienceData();
            data.AddValue("privacy", "Public");
            data.AddValue("title", "being lost");
            data.AddValue("description", "being lost");

            var query = new AddExperience(_userId, _experienceId, data);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheExperienceShouldBeStored()
        {
            SessionScope(context =>
            {
                var experience = context.Session.Load<Experience>(_experienceId);
                experience.ShouldNotBeNull();
                experience.UserId.ShouldBe((Guid) _userId);
                experience.Privacy.Level.ShouldBe(PrivacyLevel.Public);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_experienceId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<ExperiencePrivacyLevelChanged>();

                @event.UserId.ShouldBe(_userId);
                @event.ExperienceId.ShouldBe(_experienceId);
            });
        }
    }
}
