using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Documents
{
    public class WhenCreatingADocument : ApiTest
    {
        private DocumentDetails _result;
        private ShortGuid _organisationId;
        private TestAccount _account;

        public WhenCreatingADocument(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Documents.Create(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Documents.Get(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.DocumentId.ShouldBe(_organisationId);
        }
    }
}