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
    public class WhenQueryingUserByEmail : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private Messages.Users.User _result;
        private UserByEMailQuery _query;

        public WhenQueryingUserByEmail(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            var name = new Name(Guid.NewGuid().ToString("n"));
            var email = new EMail(name + "@future.now");
            var password = new Password("A-" + Guid.NewGuid().ToString("n"));

            var command = new RegisterUserCommand(name, name, email, password, password);

            context.Service.ExecuteNowWithTimeout(command);

            _query = new UserByEMailQuery(null, email);

            _result = context.Service.ExecuteNowWithTimeout(_query);
        }

        [Fact]
        public void ThenTheRefreshTokensShouldBeReturned()
        {
            _result.ShouldNotBeNull();
        }
    }
}