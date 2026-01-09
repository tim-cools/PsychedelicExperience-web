using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.ExperienceView
{
    public class WhenGettingExperiencesWithPagingSecondPage : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly UserId _userId = UserId.New();

        private ExperiencesResult _result;

        public WhenGettingExperiencesWithPagingSecondPage(PsychedelicsIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddAdministrator(_userId);

            for (var index = 0; index < 80; index++)
            {
                AddExperience(context, index.ToString(), index.ToString(), "include" + (index % 2 == 1 ? "1" : ""));
            }
        }

        private ExperienceId AddExperience(TestContext<IMediator> context, string title, string description, string tag)
        {
            return context
                .BuildExperience(_userId, title, description, _testOutputHelper)
                .WithDose(Substances.LSD)
                .WithTag(new Name(tag))
                .Id;
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetExperiences(_userId, tags: new[] { "include" }, page: 1);
            _result = context.Service.ExecuteNowWithTimeout(query, _testOutputHelper);
        }

        [Fact]
        public void ThenOnePageShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Experiences.Length.ShouldBe(20);
        }

        [Fact]
        public void ThenTheSecond20ItemsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            for (var index = 0; index < 20; index++)
            {
                _result.Experiences[index].Title.ShouldBe(((index + 20) * 2).ToString());
            }
        }

        [Fact]
        public void ThenTheTotalShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Total.ShouldBe(40);
        }
    }
}