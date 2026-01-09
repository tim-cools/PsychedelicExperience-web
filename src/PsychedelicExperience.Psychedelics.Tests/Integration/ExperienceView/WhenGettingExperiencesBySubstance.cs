using System.Collections.Generic;
using System.Linq;
using Baseline;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.ExperienceView
{
    public class WhenGettingExperiencesBySubstance : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly UserId _userId = UserId.New();
        private readonly IList<ExperienceId> _included = new List<ExperienceId>();
        private readonly IList<ExperienceId> _excluded = new List<ExperienceId>();

        private ExperiencesResult _result;

        public WhenGettingExperiencesBySubstance(PsychedelicsIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddAdministrator(_userId);

            _included.Add(AddExperience(context, Substances.LSD));
            _included.Add(AddExperience(context, Substances.LSD));

            _excluded.Add(AddExperience(context, Substances.DMT));
            _excluded.Add(AddExperience(context, Substances.DMT));
        }

        private ExperienceId AddExperience(TestContext<IMediator> context, string substance)
        {
            return context
                .BuildExperience(_userId, "With " + substance, "And " + substance, _testOutputHelper)
                .WithDose(substance)
                .Id;
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetExperiences(_userId, substances: new [] { Substances.LSD });
            _result = context.Service.ExecuteNowWithTimeout(query, _testOutputHelper);
        }

        [Fact]
        public void ThenExperiencesShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Experiences.Length.ShouldBe(2);

            _included.Each(value => _result.Experiences.Select(result => result.ExperienceId).ShouldContain(new ShortGuid(value.Value)));
        }

        [Fact]
        public void ThenTheOtherExperiencesShouldNotBeReturned()
        {
            _result.ShouldNotBeNull();

            _excluded.Each(value => _result.Experiences.Select(result => result.ExperienceId).ShouldNotContain(value));
        }
    }
}
