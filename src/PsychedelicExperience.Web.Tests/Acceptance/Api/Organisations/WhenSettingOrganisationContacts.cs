using System;
using System.Linq;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics;
using PsychedelicExperience.Psychedelics.Organisations;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenSettingAnOrganisationEmail : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenSettingAnOrganisationEmail(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.UpdateContact(_account, _organisationId, ContactTypes.EMail, "new@email.com");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenContactShouldBeSet()
        {
            Execute.WithTimeOut(
                () => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Contacts.Any(contact => contact.Type == ContactTypes.EMail
                    && contact.Value == "new@email.com"), 
                organisation => "Cound not find contact");
        }
    }

    public class WhenRemovingAnOrganisationEmail : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenRemovingAnOrganisationEmail(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);
            ApiClient.Organisations.UpdateContact(_account, _organisationId, ContactTypes.EMail, "new2@email.com");
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.DeleteContact(_account, _organisationId, ContactTypes.EMail, "new2@email.com");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenContactShouldBeRemoved()
        {
            Execute.WithTimeOut(() =>ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Contacts.All(contact => contact.Type != ContactTypes.EMail),
                details => "Contact not deleted");
        }
    }
}