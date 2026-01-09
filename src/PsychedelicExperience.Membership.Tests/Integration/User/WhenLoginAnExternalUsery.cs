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
    public class WhenLoginAnExternalUser : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private LoginResult _result;
        private string _userName;

        public WhenLoginAnExternalUser(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userName = Guid.NewGuid().ToString("n");
            var externalIdentifier = Guid.NewGuid().ToString("n");

            var command = new ExternalLoginCommand("Facebook",
                new Name(_userName), 
                externalIdentifier, 
                new EMail(_userName + "@fbi-cia-nsa.us"));
            _result = context.Service.ExecuteNowWithTimeout(command);
        }

        [Fact]
        public void ThenTheLoginShouldSucceed()
        {
            _result.Succeeded.ShouldBeTrue(_result.ToString());
        }

        [Fact]
        public void ThenTheUserShouldExist()
        {
            SessionScope((context) =>
            {
                var user = context.Session.Load<Users.Domain.User>(_result.UserId.Value);
                user.ShouldNotBeNull();
                user.DisplayName.ShouldBe(_userName);
            });
        }
    }

    public class WhenLoginAnExternalUserGivenExistingExternalUser : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private LoginResult _result;
        private string _userName;
        private string _externalIdentifier;
        private UserId _userId;
        private ExternalLoginCommand _command;

        public WhenLoginAnExternalUserGivenExistingExternalUser(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _userName = Guid.NewGuid().ToString("n");
            _externalIdentifier = Guid.NewGuid().ToString("n");

            _command = new ExternalLoginCommand("Facebook", 
                new Name(_userName),
                _externalIdentifier, 
                new EMail(_userName + "@fbi-cia-nsa.us"));
            var result = context.Service.ExecuteNowWithTimeout(_command);

            result.Succeeded.ShouldBeTrue(result.ToString);

            _userId = result.UserId;
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = context.Service.ExecuteNowWithTimeout(_command);
        }

        [Fact]
        public void ThenTheLoginShouldSucceed()
        {
            _result.Succeeded.ShouldBeTrue(_result.ToString);
        }

        [Fact]
        public void ThenTheSameUserShouldBeReturned()
        {
            _userId.ShouldBe(_result.UserId);
        }
    }

    public class WhenLoginAnExternalUserGivenExistingUser : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private LoginResult _result;
        private Name _userName;
        private string _externalIdentifier;

        public WhenLoginAnExternalUserGivenExistingUser(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _userName = new Name(Guid.NewGuid().ToString("n"));
            _externalIdentifier = Guid.NewGuid().ToString("n");

            var password = new Password("A-" + Guid.NewGuid().ToString("n"));

            var command = new RegisterUserCommand(
                _userName,
                _userName,
                new EMail(_userName  + "@fbi-cia-nsa.us"),
                password,
                password);
            var result = context.Service.ExecuteNowWithTimeout(command);

            result.Succeeded.ShouldBeTrue(result.ToString);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var command = new ExternalLoginCommand(
                "Facebook", 
                new Name("otherUserName"), 
                _externalIdentifier, 
                new EMail(_userName + "@fbi-cia-nsa.us"));
            _result = context.Service.ExecuteNowWithTimeout(command);
        }

        [Fact]
        public void ThenTheLoginShouldSucceed()
        {
            _result.Succeeded.ShouldBeTrue(_result.ToString);
        }

        [Fact]
        public void ThenTheUserNameShouldBeTheOriginalUserName()
        {
            _result.DisplayName.ShouldBe(_userName);
        }
    }

    public class WhenLoginAnExternalUserGivenExistingExternalUserFromOtherProvider : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private LoginResult _result;
        private Name _userName;
        private string _externalIdentifier;
        private UserId _userId;
        private EMail _email;

        public WhenLoginAnExternalUserGivenExistingExternalUserFromOtherProvider(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _userName = new Name(Guid.NewGuid().ToString("n"));
            _email = new EMail(_userName + "@fbi-cia-nsa.us");
            _externalIdentifier = Guid.NewGuid().ToString("n");

            var command = new ExternalLoginCommand(
                "Facebook", 
                _userName, _externalIdentifier, 
                _email);
            var result = context.Service.ExecuteNowWithTimeout(command);

            result.Succeeded.ShouldBeTrue(result.ToString);

            _userId = result.UserId;
        }

        protected override void When(TestContext<IMediator> context)
        {
            var command = new ExternalLoginCommand(
                "Google", _userName, 
                _externalIdentifier,
                _email);
            _result = context.Service.ExecuteNowWithTimeout(command);
        }

        [Fact]
        public void ThenTheLoginShouldSucceed()
        {
            _result.Succeeded.ShouldBeTrue(_result.ToString);
        }

        [Fact]
        public void ThenTheSameUserShouldBeReturned()
        {
            _userId.ShouldBe(_result.UserId);
        }
    }
}