using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    [Trait("category", "acceptance")]
    public class WhenUnlinkingAnOrganisation : ApiTest
    {
        private Guid _organisationId;
        private Guid _organisationTargetId;
        private TestAccount _account;

        public WhenUnlinkingAnOrganisation(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);
            _organisationTargetId = ApiClient.Organisations.Create(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Organisations.Link(_account, _organisationId, _organisationTargetId, "no reason");
            ApiClient.Organisations.Unlink(_account, _organisationId, _organisationTargetId, "no reason");
        }

        [Fact]
        public void ThenRelationShouldBeRemoved()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Relations.Length == 0,
                organisation => "not changed");
        }

        [Fact]
        public void ThenTargetRelationShouldBeRemoved()
        {
            Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationTargetId),
                organisation => organisation.Relations.Length == 0,
                organisation => "not changed");
        }
    }
}