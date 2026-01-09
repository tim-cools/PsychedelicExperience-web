using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenSettingAnOrganisationInfo : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenSettingAnOrganisationInfo(WebIntegrationTestFixture fixture)
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
            ApiClient.Organisations.SetInfo(_account, _organisationId, "info title", "info content");
        }


        [Fact]
        [Trait("category", "acceptance")]
        public void ThenInfoShouldBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Info != null
                    && organisation.Info.Title == "info title"
                    && organisation.Info.Content == "info content",
                organisation => "not changed");
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

    public class WhenRemovingAnOrganisationInfo : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenRemovingAnOrganisationInfo(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);
            ApiClient.Organisations.SetInfo(_account, _organisationId, "info title", "info content");
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.DeleteInfo(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenInfoShouldNotBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Info == null,
                organisation => "not changed");
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

    public class WhenSettingAnOrganisationInfoAfterRemove : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenSettingAnOrganisationInfoAfterRemove(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);

            ApiClient.Organisations.SetInfo(_account, _organisationId, "info title", "info content");
            ApiClient.Organisations.DeleteInfo(_account, _organisationId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.SetInfo(_account, _organisationId, "info title 2", "info content 2");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenInfoShouldBeSet()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Info != null
                    && organisation.Info.Title == "info title 2"
                    && organisation.Info.Content == "info content 2",
                organisation => "not changed");
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
}