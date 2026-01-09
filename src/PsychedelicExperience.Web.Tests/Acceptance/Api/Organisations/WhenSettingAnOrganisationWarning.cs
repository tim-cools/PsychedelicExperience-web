using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenSettingAnOrganisationWarning : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenSettingAnOrganisationWarning(WebIntegrationTestFixture fixture)
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
            ApiClient.Organisations.SetWarning(_account, _organisationId, "warning title", "warning content");
        }


        [Fact]
        [Trait("category", "acceptance")]
        public void ThenWarningShouldBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Warning != null
                    && organisation.Warning.Title == "warning title"
                    && organisation.Warning.Content == "warning content",
                organisation => "not changed");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenInfoShouldNotBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Info == null,
                organisation => "not changed");
        }
    }

    public class WhenRemovingAnOrganisationWarning : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenRemovingAnOrganisationWarning(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);
            ApiClient.Organisations.SetWarning(_account, _organisationId, "warning title", "warning content");
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.DeleteWarning(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenInfoShouldNotBeSet()
        {
            var organisation = ApiClient.Organisations.Get(_account, _organisationId);
            organisation.Info.ShouldBeNull();
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenWarningShouldNotBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Warning == null,
                organisation => "not changed");
        }
    }

    public class WhenSettingAnOrganisationWarningAfterRemove : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenSettingAnOrganisationWarningAfterRemove(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);

            ApiClient.Organisations.SetWarning(_account, _organisationId, "warning title", "warning content");
            ApiClient.Organisations.DeleteWarning(_account, _organisationId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.SetWarning(_account, _organisationId, "warning title 2", "warning content 2");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenWarningShouldBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Warning != null
                    && organisation.Warning.Title == "warning title 2"
                    && organisation.Warning.Content == "warning content 2",
                organisation => "not changed");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenInfoShouldNotBeSet()
        {
            var organisation = ApiClient.Organisations.Get(_account, _organisationId);
            organisation.Info.ShouldBeNull();
        }
    }
}