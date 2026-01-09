using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenGettingOrganisations : ApiTest
    {
        private JsonOrganisationsResult _result;
        private TestAccount _account;

        public WhenGettingOrganisations(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.Organisations.Create(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = (JsonOrganisationsResult) ApiClient.Organisations.GetList(_account);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.Total.ShouldBeGreaterThan(0);
        }
    }

    public class WhenGettingOrganisationsCsv : ApiTest
    {
        private CsvOrganisationsResult _result;
        private TestAccount _account;

        public WhenGettingOrganisationsCsv(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.Organisations.Create(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = (CsvOrganisationsResult) ApiClient.Organisations.GetList(_account, format: Format.Csv );
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenResultShouldContainValue()
        {
            _result.Bytes.Length.ShouldBePositive();
        }
    }

    public class WhenGettingOrganisationsByType : ApiTest
    {
        private JsonOrganisationsResult _result;
        private TestAccount _account;
        private OrganisationDetails _organisation;

        public WhenGettingOrganisationsByType(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);

            var organisationId = ApiClient.Organisations.Create(_account);

            _organisation = ApiClient.Organisations.Get(_account, organisationId);

            ApiClient.Organisations.AddType(_account, organisationId, "Retreat");

            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, organisationId),
                details => details.Types.Length > 0,
                details => "not set");
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = (JsonOrganisationsResult) ApiClient.Organisations.GetList(_account, query: _organisation.Name, types: new [] {  "Retreat" });
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.Total.ShouldBe(1);
            _result.Organisations[0].OrganisationId.ShouldBe(_organisation.OrganisationId);
        }
    }

    public class WhenGettingOrganisationsByOtherType : ApiTest
    {
        private JsonOrganisationsResult _result;
        private TestAccount _account;
        private OrganisationDetails _organisation;

        public WhenGettingOrganisationsByOtherType(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);

            var organisationId = ApiClient.Organisations.Create(_account);

            _organisation = ApiClient.Organisations.Get(_account, organisationId);

            ApiClient.Organisations.AddType(_account, organisationId, "Retreat");
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = (JsonOrganisationsResult) ApiClient.Organisations.GetList(_account, query: _organisation.Name, types: new [] {  "Clinic" });
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenNoValueShouldBeReturned()
        {
            _result.Total.ShouldBe(0);
        }
    }

    public class WhenGettingOrganisationsByMultipleTypes : ApiTest
    {
        private JsonOrganisationsResult _result;
        private TestAccount _account;
        private OrganisationDetails _organisation1;
        private OrganisationDetails _organisation2;
        private string _name;

        public WhenGettingOrganisationsByMultipleTypes(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);

            _name = ShortGuid.NewGuid().Value;

            var organisationId1 = ApiClient.Organisations.Create(_account, name: _name);
            var organisationId2 = ApiClient.Organisations.Create(_account, name: _name);

            _organisation1 = ApiClient.Organisations.Get(_account, organisationId1);
            _organisation2 = ApiClient.Organisations.Get(_account, organisationId2);

            ApiClient.Organisations.AddType(_account, organisationId1, "Retreat");
            ApiClient.Organisations.AddType(_account, organisationId2, "Clinic");

            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, organisationId1),
                details => details.Types.Length == 1,
                details => "not set");
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, organisationId2),
                details => details.Types.Length == 1,
                details => "not set");
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = (JsonOrganisationsResult) ApiClient.Organisations.GetList(_account, query: _name, types: new [] { "Clinic", "Retreat" });
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenNoValueShouldBeReturned()
        {
            _result.Total.ShouldBe(2);
            _result.Organisations
                .ShouldContain(organisation => organisation.OrganisationId == _organisation1.OrganisationId);
            _result.Organisations
                .ShouldContain(organisation => organisation.OrganisationId == _organisation2.OrganisationId);
        }
    }

    public class WhenGettingOrganisationsByMultipleDifferentTypes : ApiTest
    {
        private JsonOrganisationsResult _result;
        private TestAccount _account;
        private OrganisationDetails _organisation1;
        private OrganisationDetails _organisation2;
        private string _name;

        public WhenGettingOrganisationsByMultipleDifferentTypes(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);

            _name = ShortGuid.NewGuid().Value;

            var organisationId1 = ApiClient.Organisations.Create(_account, name: _name);
            var organisationId2 = ApiClient.Organisations.Create(_account, name: _name);

            _organisation1 = ApiClient.Organisations.Get(_account, organisationId1);
            _organisation2 = ApiClient.Organisations.Get(_account, organisationId2);

            ApiClient.Organisations.AddType(_account, organisationId1, "Retreat");
            ApiClient.Organisations.AddType(_account, organisationId1, "Clinic");
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = (JsonOrganisationsResult) ApiClient.Organisations.GetList(_account, query: _name, types: new [] {  "Community", "Research" });
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenNoValueShouldBeReturned()
        {
            _result.Total.ShouldBe(0);
        }
    }
}