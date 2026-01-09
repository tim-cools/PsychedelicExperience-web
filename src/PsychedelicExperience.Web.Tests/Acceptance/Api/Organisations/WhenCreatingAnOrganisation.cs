using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using Shouldly;
using Xunit;
using Contact = PsychedelicExperience.Psychedelics.Messages.Organisations.Contact;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenCreatingAnOrganisation : ApiTest
    {
        private OrganisationDetails _result;
        private ShortGuid _organisationId;
        private TestAccount _account;

        public WhenCreatingAnOrganisation(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            _organisationId = ApiClient.Organisations.Create(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Organisations.Get(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.OrganisationId.ShouldBe(_organisationId);
            _result.Person.ShouldBeFalse();
        }
    }

    public class WhenCreatingAnOrganisationAsPerson : ApiTest
    {
        private OrganisationDetails _result;
        private ShortGuid _organisationId;
        private TestAccount _account;

        public WhenCreatingAnOrganisationAsPerson(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            _organisationId = ApiClient.Organisations.Create(_account, person: true);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Organisations.Get(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.OrganisationId.ShouldBe(_organisationId);
            _result.Person.ShouldBeTrue();
        }
    }

    public class WhenCreatingAnOrganisationAsCenter : ApiTest
    {
        private readonly Center _center = TestData.Center();
        private OrganisationDetails _result;
        private ShortGuid _organisationId;
        private TestAccount _account;

        public WhenCreatingAnOrganisationAsCenter(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            _organisationId = ApiClient.Organisations.Create(_account, center: _center);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Organisations.Get(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.OrganisationId.ShouldBe(_organisationId);
            _result.Center.GroupSize.Maximum.ShouldBeDeepEqual(_center.GroupSize.Maximum);
        }
    }

    public class WhenCreatingAnOrganisationWithContacts : ApiTest
    {
        private ShortGuid _organisationId;
        private TestAccount _account;

        public WhenCreatingAnOrganisationWithContacts(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();

        }

        protected override void When(TestContext<IMediator> context)
        {
            var contacts = new[]
            {
                new Contact { Type = ContactTypes.EMail, Value = "some@fbi-cia-nsa.us"},
                new Contact { Type = ContactTypes.Phone, Value = "+32 486 789 789"},
                new Contact { Type = ContactTypes.WebSite, Value = "https://mailurl"},
            };

            _organisationId = ApiClient.Organisations.Create(_account, contacts: contacts);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentOrganisationShouldContainThePhone()
        {
            Execute.WithTimeOut(
                () => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Contacts.Any(contact => contact.Type == ContactTypes.Phone 
                            && contact.Value == "+32 486 789 789"),
                organisation => "Cound not find Phone");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentOrganisationShouldContainTheEmail()
        {
            Execute.WithTimeOut(
                () => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Contacts.Any(contact => contact.Type == ContactTypes.EMail
                    && contact.Value == "some@fbi-cia-nsa.us"),
                organisation => "Cound not find EMail");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentOrganisationShouldContainTheWebsite()
        {
            Execute.WithTimeOut(
                () => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Contacts.Any(contact => contact.Type == ContactTypes.WebSite
                    && contact.Value == "https://mailurl"),
                organisation => "Cound not find WebSite");
        }
    }
}