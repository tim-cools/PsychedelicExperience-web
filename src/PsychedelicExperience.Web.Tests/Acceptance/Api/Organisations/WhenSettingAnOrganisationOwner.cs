using System;
using System.Threading;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenSettingAnOrganisationOwner : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;
        private TestAccount _owner;

        public WhenSettingAnOrganisationOwner(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _owner = ApiClient.Account.CreateTestAccount();
            _organisationId = ApiClient.Organisations.Create(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.SetOwner(_account, _organisationId, _owner.EMail);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenOwnerShouldBeSet()
        {
            Execute.WithTimeOut(
                () => ApiClient.Organisations.Get(_owner, _organisationId),
                organisation => organisation.Privileges.IsOwner,
                organisation => "Owner not set");
        }
    }

    public class WhenRemovingAnOrganisationOwner : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;
        private TestAccount _owner;

        public WhenRemovingAnOrganisationOwner(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _owner = ApiClient.Account.CreateTestAccount();
            _organisationId = ApiClient.Organisations.Create(_account);

            ApiClient.Organisations.SetOwner(_account, _organisationId, _owner.EMail);
            Execute.WithTimeOut(
                () => ApiClient.Organisations.Get(_owner, _organisationId),
                organisation => organisation.Privileges.IsOwner,
                organisation => "Owner not set");
        }

        protected override void When(TestContext<IMediator> context)
        {
            Thread.Sleep(10000);
            ApiClient.Organisations.RemoveOwner(_account, _organisationId, _owner);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenOwnerShouldBeSet()
        {
            Execute.WithTimeOut(
                () => ApiClient.Organisations.Get(_owner, _organisationId),
                organisation => !organisation.Privileges.IsOwner,
                organisation => "Owner not reset");
        }
    }
}