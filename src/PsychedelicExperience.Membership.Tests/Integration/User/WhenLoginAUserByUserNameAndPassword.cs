using System;
using Shouldly;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Integration.User
{
    public class WhenLoginAUserByUserNameAndPasswordGivenUseDoesNotExists : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private LoginResult _result;

        public WhenLoginAUserByUserNameAndPasswordGivenUseDoesNotExists(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            var userName = new EMail(Guid.NewGuid().ToString("n"));
            var password = new Password(Guid.NewGuid().ToString("n"));

            var command = new UserEMailPasswordLoginCommand(userName, password);
            _result = context.Service.ExecuteNowWithTimeout(command);
        }

        [Fact]
        public void ThenTheRefreshTokensShouldBeReturned()
        {
            _result.Succeeded.ShouldBeFalse();
        }
    }
}