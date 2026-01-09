using System;
using System.Linq;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using Shouldly;
using Xunit;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Doses
{
    public class WhenUpdatingADoseNotes : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();

        private DoseId _doseId;
        private ExperienceId _experienceId;
        private Result _result;

        public WhenUpdatingADoseNotes(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _experienceId = context.AddExperience(_userId);
            _doseId = context.AddDose(_userId, _experienceId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new UpdateDoseNotes(_userId, _doseId, "5-meo-dmt");
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheDoseShouldBeUpdated()
        {
            SessionScope(context =>
            {
                Test.All(_ => _.IsNotNull(context.Session.Load<Dose>(_doseId), (dose, __) => __
                    .AreEqual(dose.Id, _doseId.Value)
                    .AreEqual(dose.Notes, "5-meo-dmt")));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_doseId).ToArray();
                events.Length.ShouldBe(2, () => JsonConvert.SerializeObject(events));

                Test.All(_ => _.IsNotNull(events.LastEventShouldBeOfType<DoseNotesUpdated>(), (@event, __) => __
                    .AreEqual(@event.DoseId, _doseId, "doseId")
                    .AreEqual(@event.UserId, _userId, "userId")
                    .AreEqual(@event.Notes, "5-meo-dmt")));
            });
        }
    }
}