using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.Users;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Account
{
    public class WhenAddingUserRoleGivenWeGetIsAsANormalUser : ApiTest
    {
        private TestAccount _account;
        private TestAccount _administrator;

        public WhenAddingUserRoleGivenWeGetIsAsANormalUser(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _administrator = ApiClient.Account.GetAdministrator(context);
            _account = ApiClient.Account.CreateTestAccount();
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Account.AddRoleToUser(_administrator, _account, Role.ContentManager);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenShouldHaveTheRole()
        {
            Execute.WithTimeOut(
                () => ApiClient.UserProfile.Get(_account),
                profile => profile?.Roles == null,
                organisation => "User profile not updated");
        }
    }
    public class WhenAddingUserRole : ApiTest
    {
        private TestAccount _account;
        private TestAccount _administrator;

        public WhenAddingUserRole(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _administrator = ApiClient.Account.GetAdministrator(context);
            _account = ApiClient.Account.CreateTestAccount();
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.Account.AddRoleToUser(_administrator, _account, Role.ContentManager);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenShouldHaveTheRole()
        {
            var user = Execute.WithTimeOut(
                () =>  ApiClient.UserProfile.Get(_administrator, _account),
                profile => profile?.Roles != null && profile.Roles.Length == 1,
                organisation => "User profile not updated");

            user.Roles[0].ShouldBe(Role.ContentManager.ToString());
        }
    }
}