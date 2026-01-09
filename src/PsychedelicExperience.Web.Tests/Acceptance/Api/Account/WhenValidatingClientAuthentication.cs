using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.Users;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Account
{
    public class WhenGettingTheCurrentProfile : ApiTest
    {
        private TestAccount _account;
        private User _result;

        public WhenGettingTheCurrentProfile(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            _result = ApiClient.Account.GetAccount(_account);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.Name.ShouldBe(_account.DisplayName);
        }
    }
}