using System;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Doses
{
    public class WhenUpdatingADoseUnit : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();

        private DoseId _doseId;
        private ExperienceId _experienceId;
        private Result _result;

        public WhenUpdatingADoseUnit(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _experienceId = context.AddExperience(_userId, "Mild effect");
            _doseId = context.AddDose(_userId, _experienceId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new UpdateDoseUnit(_userId, _doseId, "g");
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
                    .AreEqual(dose.Id, (Guid) _doseId)
                    .AreEqual(dose.Unit, "g")));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_doseId);
                events.Count.ShouldBe(2, () => JsonConvert.SerializeObject(events));

                Test.All(_ => _.IsNotNull(events.LastEventShouldBeOfType<DoseUnitUpdated>(), (@event, __) => __
                    .AreEqual(@event.DoseId, _doseId, "doseId")
                    .AreEqual(@event.UserId, _userId, "userId")
                    .AreEqual(@event.Unit, "g")));
            });
        }
    }
}