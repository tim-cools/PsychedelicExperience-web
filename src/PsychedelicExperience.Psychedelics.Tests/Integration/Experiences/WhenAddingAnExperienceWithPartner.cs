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
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Experiences
{
    public class WhenAddingAnExperienceWithPartner : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly ExperienceId _experienceId = ExperienceId.New();
        private Result _result;

        public WhenAddingAnExperienceWithPartner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            var data = new ExperienceData();
            data.AddValue("partner", "TYPM");
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
                experience.Partners.ShouldContain(value => value.Value == "TYPM");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_experienceId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<ExperienceAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.ExperienceId.ShouldBe(_experienceId);
            });
        }
    }

    public class WhenAddingAnExperienceWithDate : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly ExperienceId _experienceId = ExperienceId.New();
        private Result _result;

        public WhenAddingAnExperienceWithDate(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            var data = new ExperienceData();
            data.AddValue("title", "being lost");
            data.AddValue("date", "2019-08-07T16:30:00+02:00");

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
                experience.DateTime.Value.Date.ShouldBe(new DateTime(2019, 08, 07));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_experienceId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<ExperienceAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.ExperienceId.ShouldBe(_experienceId);
            });
        }
    }
}