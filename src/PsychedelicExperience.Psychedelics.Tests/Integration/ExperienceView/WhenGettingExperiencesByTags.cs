using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.ExperienceView
{
    public class WhenGettingExperiencesByTags : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly UserId _userId = UserId.New();
        private readonly List<ExperienceId> _included = new List<ExperienceId>();
        private readonly List<ExperienceId> _excluded = new List<ExperienceId>();

        private ExperiencesResult _result;

        public WhenGettingExperiencesByTags(PsychedelicsIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddAdministrator(_userId);

            _included.Add(AddExperience(context, "include1", "include2"));

            _excluded.Add(AddExperience(context, "include1"));
            _excluded.Add(AddExperience(context, "include2"));
            _excluded.Add(AddExperience(context, "exclude1"));
            _excluded.Add(AddExperience(context, "exclude2"));
        }

        private ExperienceId AddExperience(TestContext<IMediator> context, params string[] tags)
        {
            return context
                .BuildExperience(_userId, "With " + tags.Length, "And " + tags.Length, _testOutputHelper)
                .WithDose(Substances.LSD)
                .WithTags(tags.Select(value => new Name(value)).ToArray())
                .Id;
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetExperiences(_userId, tags: new[] { "include1", "include2" });
            _result = context.Service.ExecuteNowWithTimeout(query, _testOutputHelper);
        }

        [Fact]
        public void ThenExperiencesShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Experiences.Length.ShouldBe(1);

            _included.ForEach(value => _result.Experiences.Select(result => result.ExperienceId).ShouldContain(new ShortGuid(value.Value)));
        }

        [Fact]
        public void ThenTheOtherExperiencesShouldNotBeReturned()
        {
            _result.ShouldNotBeNull();

            _excluded.ForEach(value => _result.Experiences.Select(result => result.ExperienceId).ShouldNotContain(new ShortGuid(value.Value)));
        }
    }
}