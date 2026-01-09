using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.OrganisationsView
{
    public class WhenGettingOrganisationsByTagDifferentCase : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly UserId _userId = UserId.New();
        private readonly List<ShortGuid> _included = new List<ShortGuid>();
        private readonly List<ShortGuid> _excluded = new List<ShortGuid>();

        private JsonOrganisationsResult _result;

        private const string _include = "Shop";
        private const string _exclude = "Conference";
        private const string _exclude2 = "Religious";

        public WhenGettingOrganisationsByTagDifferentCase(PsychedelicsIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddAdministrator(_userId);

            _included.Add(AddOrganisation(context, _include));
            _included.Add(AddOrganisation(context, _include));

            _excluded.Add(AddOrganisation(context, _exclude));
            _excluded.Add(AddOrganisation(context, _exclude2));
        }

        private Guid AddOrganisation(TestContext<IMediator> context, string tag)
        {
            return (Guid) context
                .BuildOrganisation(_userId, "With " + tag, _testOutputHelper)
                .WithTag(tag)
                .Id;
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetOrganisations(_userId, tags: new[] { _include.ToUpperInvariant() });
            _result = (JsonOrganisationsResult) context.Service.ExecuteNowWithTimeout(query, _testOutputHelper);
        }

        [Fact]
        public void ThenOrganisationsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Organisations.Length.ShouldBe(2);

            _included.ForEach(value => _result.Organisations.Select(result => result.OrganisationId).ShouldContain(value));
        }

        [Fact]
        public void ThenTheOtherOrganisationsShouldNotBeReturned()
        {
            _result.ShouldNotBeNull();

            _excluded.ForEach(value => _result.Organisations.Select(result => result.OrganisationId).ShouldNotContain(value));
        }
    }
}