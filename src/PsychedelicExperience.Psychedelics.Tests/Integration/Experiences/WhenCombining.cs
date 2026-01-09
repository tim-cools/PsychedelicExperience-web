using System;
using System.Linq;
using Newtonsoft.Json;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using Shouldly;
using Xunit;
using ExperienceLevel = PsychedelicExperience.Psychedelics.Experiences.ExperienceLevel;
using PrivacyLevel = PsychedelicExperience.Psychedelics.Experiences.PrivacyLevel;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Experiences
{
    public class WhenCombining : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private readonly Name _tagName = new Name(Guid.NewGuid().ToString("N"));
        private ExperienceId _experienceId;

        public WhenCombining(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _experienceId = context.AddExperience(_userId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            context
                .ShouldSucceed(new UpdateExperienceTitle(_experienceId, _userId, new Title("title")))
                .ShouldSucceed(new SetExperienceLevel(_experienceId, _userId, Messages.Experiences.ExperienceLevel.Level3))
                .ShouldSucceed(new AddExperienceTag(_experienceId, _userId, _tagName))
                .ShouldSucceed(new SetExperiencePrivacy(_experienceId, _userId, Messages.Experiences.PrivacyLevel.Restricted))
                .ShouldSucceed(new UpdateExperiencePublicDescription(_experienceId, _userId, new Description("new-description")));
        }

        [Fact]
        public void ThenTheExperiencehoudlBeUpdated()
        {
            SessionScope(context =>
            {
                Test.All(_ => _.IsNotNull(context.Session.Load<Experience>(_experienceId), (experience, __) => __
                    .AreEqual(experience.Title, new Title("title"))
                    .AreEqual(experience.Level, ExperienceLevel.Level3)
                    .IsNotNull(experience.Tags.FirstOrDefault(where => Equals(@where.Name, _tagName)))
                    .AreEqual(experience.Privacy.Level, PrivacyLevel.Restricted)
                    .AreEqual(experience.Public.Description, new Description("new-description"))
                    ));
            });
        }

        [Fact]
        public void ThenTheEventsShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_experienceId).ToArray();
                events.Length.ShouldBe(6, () => JsonConvert.SerializeObject(events));
            });
        }
    }
}