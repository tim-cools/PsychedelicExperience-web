using System.Linq;
using Newtonsoft.Json;
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
    public class WhenRemovingAnExperience : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private ExperienceId _experienceId;
        private Result _result;

        public WhenRemovingAnExperience(PsychedelicsIntegrationTestFixture fixture)
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
            var query = new RemoveExperience(_experienceId, _userId );
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        //[Fact]
        //public void ThenTheExperienceShouldBeStored()
        //{
        //    SessionScope(context =>
        //    {
        //        var experience = context.Session.Load<Experience>(_experienceId);
        //        experience.ShouldBeNull();
        //    });
        //}

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_experienceId).ToArray();
                events.Length.ShouldBe(2, () => JsonConvert.SerializeObject(events));

                Test.All(_ => _.IsNotNull(events.LastEventShouldBeOfType<ExperienceRemoved>(), (@event, __) => __
                    .AreEqual(@event.UserId, _userId)
                    .AreEqual(@event.ExperienceId, _experienceId)));
            });
        }
    }
}
