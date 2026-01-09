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
    public class WhenAddingADose : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly DoseId _doseId = DoseId.New();
        private UserId _userId;
        private ExperienceId _experienceId;
        private Result _result;

        public WhenAddingADose(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _experienceId = context.AddExperience(_userId, "What else");
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new AddDose(_experienceId, _userId, _doseId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheDoseShouldBeStored()
        {
            SessionScope(context =>
            {
                Test.All(_ => _.IsNotNull(context.Session.Load<Dose>(_doseId.Value), (dose, __) => __
                    .AreEqual(dose.Id, _doseId.Value)));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_doseId);
                events.Count.ShouldBe(1, () => JsonConvert.SerializeObject(events));

                Test.All(_ => _.IsNotNull(events.LastEventShouldBeOfType<DoseAdded>(), (@event, __) => __
                    .AreEqual(@event.DoseId, _doseId, "doseId")
                    .AreEqual(@event.UserId, _userId, "userId")
                    .AreEqual(@event.ExperienceId, _experienceId)));
            });
        }
    }
}
