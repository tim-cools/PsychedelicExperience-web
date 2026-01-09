using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenSettingAnOrganisationCenter : ApiTest
    {
        private readonly Center _center = TestData.Center();
        private Guid _organisationId;
        private TestAccount _account;

        public WhenSettingAnOrganisationCenter(WebIntegrationTestFixture fixture)
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
            ApiClient.Organisations.SetCenter(_account, _organisationId, _center);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCenterShouldBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Center != null
                    && organisation.Center.Location.Description == _center.Location.Description,
                organisation => "not changed");
        }
    }

    public class WhenRemovingAnOrganisationCenter : ApiTest
    {
        private readonly Center _center = TestData.Center();
        private Guid _organisationId;
        private TestAccount _account;

        public WhenRemovingAnOrganisationCenter(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account, center: _center);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.DeleteCenter(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCenterShouldNotBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Center == null,
                organisation => "not changed");
        }

    }

    public class WhenSettingAnOrganisationCenterAfterRemove : ApiTest
    {
        private readonly Center _center = TestData.Center();
        private Guid _organisationId;
        private TestAccount _account;

        public WhenSettingAnOrganisationCenterAfterRemove(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);

            ApiClient.Organisations.SetCenter(_account, _organisationId, _center);
            ApiClient.Organisations.DeleteCenter(_account, _organisationId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _center.Location.Description = "Outer space 2";
            ApiClient.Organisations.SetCenter(_account, _organisationId, _center);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCenterShouldBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Center != null
                    && organisation.Center.Location.Description == _center.Location.Description,
                organisation => "not changed");
        }
    }
}