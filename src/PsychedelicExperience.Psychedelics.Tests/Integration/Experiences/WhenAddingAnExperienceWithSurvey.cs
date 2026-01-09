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
using ArrayValue = PsychedelicExperience.Psychedelics.Messages.Surveys.ArrayValue;
using StringValue = PsychedelicExperience.Psychedelics.Messages.Surveys.StringValue;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Experiences
{
    public class WhenAddingAnExperienceWithSurvey : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly ExperienceId _experienceId = ExperienceId.New();
        private Result _result;

        public WhenAddingAnExperienceWithSurvey(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            var data = new ExperienceData();
            data.AddValue("title", "Why not");
            data.AddValue("description", "being lost");

            data.AddValue("survey-id", "pex-1");
            data.AddValue("integration-a", "being lost");
            data.AddValue("setting-a", "being slost");
            data.AddArray("mystical-a", "being most");
            data.AddArray("mystical-a", "&&&");

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

        [Fact]
        public void ThenTheSurveyShouldBeStored()
        {
            SessionScope(context =>
            {
                var survey = context.Session
                    .Query<Survey>()
                    .SingleOrDefault(survey => survey.ExperienceId == _experienceId.Value);

                survey.ShouldNotBeNull();
                survey.Name.ShouldBe("pex-1");
                survey.UserId.ShouldBe((Guid) _userId);
                survey.Data.Values.Count.ShouldBe(3);
                survey.Data.Values.Count.ShouldBe(3);
                survey.Data.Values.SingleOrDefault(value => value.Key == "integration-a")
                    .ShouldBeOfType<StringValue>().Value
                    .ShouldBe("being lost");
                survey.Data.Values.SingleOrDefault(value => value.Key == "setting-a")
                    .ShouldBeOfType<StringValue>().Value
                    .ShouldBe("being slost");
                survey.Data.Values.SingleOrDefault(value => value.Key == "mystical-a")
                    .ShouldBeOfType<ArrayValue>().Values
                    .ShouldBe(new[] {"being most", "&&&"});
            });
        }
    }
}
