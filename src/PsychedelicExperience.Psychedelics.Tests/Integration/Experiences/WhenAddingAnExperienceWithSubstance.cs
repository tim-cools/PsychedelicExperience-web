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

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Experiences
{
    public class WhenAddingAnExperienceWithSubstance : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly ExperienceId _experienceId = ExperienceId.New();
        private Result _result;

        public WhenAddingAnExperienceWithSubstance(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            var data = new ExperienceData();
            data.AddValue("title", "Why not");
            data.AddValue("description", "being lost");
            data.AddValue("substance", "MDMA");

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
                experience.Title.Value.ShouldBe("Why not");
                experience.Public.Description.Value.ShouldBe("being lost");
            });
        }

        [Fact]
        public void ThenTheDoseShouldBeStored()
        {
            SessionScope(context =>
            {
                var dose = context.Session
                    .Query<Dose>()
                    .SingleOrDefault(dose => dose.ExperienceId == _experienceId.Value);

                dose.ShouldNotBeNull();
                dose.UserId.ShouldBe((Guid) _userId);
                dose.Substance.ShouldBe("MDMA");
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

    public class WhenAddingAnExperienceWithSubstances : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly ExperienceId _experienceId = ExperienceId.New();
        private Result _result;

        public WhenAddingAnExperienceWithSubstances(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            var data = new ExperienceData();
            data.AddValue("title", "Why not");
            data.AddValue("description", "being lost");
            data.AddArray("substances", "MDMA");
            data.AddArray("substances", "PCP");

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
                experience.Title.Value.ShouldBe("Why not");
                experience.Public.Description.Value.ShouldBe("being lost");
            });
        }

        [Fact]
        public void ThenTheDoseShouldBeStored()
        {
            SessionScope(context =>
            {
                var doses = context.Session
                    .Query<Dose>()
                    .Where(dose => dose.ExperienceId == _experienceId.Value)
                    .ToArray();

                doses.Length.ShouldBe(2);
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
