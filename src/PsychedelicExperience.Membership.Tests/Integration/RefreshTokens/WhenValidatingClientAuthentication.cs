using System;
using Shouldly;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Clients.Handlers;
using PsychedelicExperience.Membership.Messages.RefreshTokens;
using PsychedelicExperience.Membership.Messages.Users;
using Xunit;
using ErrorCodes = PsychedelicExperience.Membership.Messages.ErrorCodes;

namespace PsychedelicExperience.Membership.Tests.Integration.RefreshTokens
{
    public class WhenValidatingRefreshTokenGivenTokenIsValid : ServiceTestBase<IMediator>,
        IClassFixture<MembershipIntegrationTestFixture>
    {
        private Result _result;
        private string _refreshToken;
        private string _clientId;
        private string _userId;

        public WhenValidatingRefreshTokenGivenTokenIsValid(MembershipIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _refreshToken = Guid.NewGuid().ToString("n");
            _clientId = Guid.NewGuid().ToString("n");
            _userId = Guid.NewGuid().ToString("n");

            var command = new CreateRefreshTokenCommand(_refreshToken, _clientId, _userId, "me", "*", DateTimeOffset.Now,
                DateTimeOffset.Now);

            var result = context.Service.ExecuteNowWithTimeout(command);
            result.Succeeded.ShouldBeTrue(result.ToString);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new RefreshTokenValidator(_refreshToken, _clientId, _userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTokenShouldBeValid()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeTrue(_result.ToString);
        }
    }

    public class WhenValidatingRefreshTokenGivenTokenDoesNotExists : ServiceTestBase<IMediator>,
       IClassFixture<MembershipIntegrationTestFixture>
    {
        private Result _result;

        public WhenValidatingRefreshTokenGivenTokenDoesNotExists(MembershipIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            var refreshToken = Guid.NewGuid().ToString("n");
            var clientId = Guid.NewGuid().ToString("n");
            var userId = Guid.NewGuid().ToString("n");

            var query = new RefreshTokenValidator(refreshToken, clientId, userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTokenShouldBeUnknown()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeFalse();
            _result.Errors.ShouldContain(error => error.Code == ErrorCodes.RefreshTokenInvalid);
        }
    }

    public class WhenValidatingRefreshTokenGivenTokenHasDifferntUser : ServiceTestBase<IMediator>,
       IClassFixture<MembershipIntegrationTestFixture>
    {
        private Result _result;
        private string _refreshToken;
        private string _clientId;

        public WhenValidatingRefreshTokenGivenTokenHasDifferntUser(MembershipIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _refreshToken = Guid.NewGuid().ToString("n");
            _clientId = Guid.NewGuid().ToString("n");
            var userId = Guid.NewGuid().ToString("n");

            var command = new CreateRefreshTokenCommand(_refreshToken, _clientId, userId, "me", "*", DateTimeOffset.Now,
                DateTimeOffset.Now);

            var result = context.Service.ExecuteNowWithTimeout(command);
            result.Succeeded.ShouldBeTrue(result.ToString);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new RefreshTokenValidator(_refreshToken, _clientId, Guid.NewGuid().ToString("n"));
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTokenShouldBeInvalid()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeFalse();
            _result.Errors.ShouldContain(error => error.Code == Messages.ErrorCodes.RefreshTokenInvalid);
        }
    }

    public class WhenValidatingRefreshTokenGivenTokenHasDifferntClient : ServiceTestBase<IMediator>,
       IClassFixture<MembershipIntegrationTestFixture>
    {
        private Result _result;
        private string _refreshToken;
        private string _userId;

        public WhenValidatingRefreshTokenGivenTokenHasDifferntClient(MembershipIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _refreshToken = Guid.NewGuid().ToString("n");
            _userId = Guid.NewGuid().ToString("n");
            var clientId = Guid.NewGuid().ToString("n");

            var command = new CreateRefreshTokenCommand(_refreshToken, clientId, _userId, "me", "*", DateTimeOffset.Now,
                DateTimeOffset.Now);

            var result = context.Service.ExecuteNowWithTimeout(command);
            result.Succeeded.ShouldBeTrue(result.ToString);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new RefreshTokenValidator(_refreshToken, Guid.NewGuid().ToString("n"), _userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTokenShouldBeInvalid()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeFalse();
            _result.Errors.ShouldContain(error => error.Code == Messages.ErrorCodes.RefreshTokenInvalid);
        }
    }
}