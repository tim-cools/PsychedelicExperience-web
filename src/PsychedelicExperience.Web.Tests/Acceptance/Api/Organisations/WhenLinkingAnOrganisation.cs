using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    [Trait("category", "acceptance")]
    public class WhenLinkingAnOrganisation : ApiTest
    {
        private Guid _organisationId;
        private Guid _organisationTargetId;
        private TestAccount _account;

        public WhenLinkingAnOrganisation(WebIntegrationTestFixture fixture)
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
            ApiClient.Organisations.Link(_account, _organisationId, _organisationTargetId, "Shaman");
        }

        [Fact]
        public void ThenRelationShouldBeAdded()
        {
            var organisation = Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation.Relations.Length == 1 ,
                organisation => "not changed");

            var target = ApiClient.Organisations.Get(_account, _organisationTargetId);

            var relation = organisation.Relations.First();
            relation.OrganisationId.ShouldBe(new ShortGuid(_organisationTargetId));
            relation.OrganisationName.ShouldBe(target.Name);
            relation.OrganisationUrl.ShouldBe(target.Url);
            relation.Relation.ShouldBe("Shaman");
        }

        [Fact]
        public void ThenTargetRelationShouldBeAdded()
        {
            var organisation = Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationTargetId),
                organisation => organisation.RelationsFrom.Length == 1 ,
                organisation => "not changed");

            var target = ApiClient.Organisations.Get(_account, _organisationId);

            var relation = organisation.RelationsFrom.First();
            relation.OrganisationId.ShouldBe(new ShortGuid(_organisationId));
            relation.OrganisationName.ShouldBe(target.Name);
            relation.OrganisationUrl.ShouldBe(target.Url);
            relation.Relation.ShouldBe("Shaman");
        }
    }
}