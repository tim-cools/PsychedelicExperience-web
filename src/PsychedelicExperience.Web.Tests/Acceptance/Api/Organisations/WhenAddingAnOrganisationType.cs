using System;
using System.Linq;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenAddingAnOrganisationType : ApiTest
    {
        private Guid _organisationId;
        private TestAccount _account;

        public WhenAddingAnOrganisationType(WebIntegrationTestFixture fixture)
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
            ApiClient.Organisations.AddType(_account, _organisationId, "Retreat");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTypeShouldBeSet()
        {
            var organisation = Execute.WithTimeOut(() => ApiClient.Organisations.Get(_account, _organisationId),
                organisation => organisation?.Types?.Length == 1,
                organisation => "not changed");

            organisation.Types.First().ShouldBe("Retreat");
        }
    }
}