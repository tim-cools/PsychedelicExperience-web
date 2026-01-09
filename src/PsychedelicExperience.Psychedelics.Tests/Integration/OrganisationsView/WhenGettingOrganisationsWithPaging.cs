using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.OrganisationsView
{
    public class WhenGettingOrganisationsWithPaging : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly UserId _userId = UserId.New();

        private JsonOrganisationsResult _result;

        private const string _include = "Shop";
        private const string _include2 = "Multimedia";


        public WhenGettingOrganisationsWithPaging(PsychedelicsIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
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
                AddOrganisation(context, $"name{index:000}", (index % 2 == 1 ? _include2 : _include));
            }
        }

        private OrganisationId AddOrganisation(TestContext<IMediator> context, string title, string tag)
        {
            return context
                .BuildOrganisation(_userId, title, _testOutputHelper)
                .WithTag(tag)
                .Id;
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetOrganisations(_userId, tags: new[] { _include });
            _result = (JsonOrganisationsResult) context.Service.ExecuteNowWithTimeout(query, _testOutputHelper);
        }

        [Fact]
        public void ThenOnePageShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Organisations.Length.ShouldBe(20);
        }

        [Fact]
        public void ThenTheFirst20ItemsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            for (var index = 0; index < 20; index++)
            {
                _result.Organisations[index].Name.ShouldBe($"name{index * 2:000}");
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