using System;
using System.Linq;
using Shouldly;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Integration.User
{
    public class WhenRegisteringAUser : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private Result _result;
        private RegisterUserCommand _command;

        public WhenRegisteringAUser(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            var name = new Name(Guid.NewGuid().ToString("n"));
            var email = new EMail("eMail@future.now");
            var password = new Password("A-123assword");

            _command = new RegisterUserCommand(name, name, email, password, password);

            _result = context.Service.ExecuteNowWithTimeout(_command);
        }

        [Fact]
        public void ThenTheResultShouldSucceed()
        {
            _result.Succeeded.ShouldBeTrue();
        }

        [Fact]
        public void ThenTheResultShouldHaveNoErrors()
        {
            if (_result.Errors.Any())
            {
                var errors = _result.Errors.Aggregate(string.Empty, (value, result) => $"{result}, {value}");
                throw new InvalidOperationException(errors);
            }
        }

        [Fact]
        public void ThenAUserShouldBeStored()
        {
            SessionScope((context) =>
            {
                var userByNameQuery = new UserByEMailQuery(null, _command.EMail);
                var user = context.Service.ExecuteNowWithTimeout(userByNameQuery);

                user.ShouldNotBeNull();
            });
        }

        [Fact]
        public void ThenAUserShouldBeAbleToLogin()
        {
            SessionScope((context) =>
            {
                var validUserLoginQuery = new UserEMailPasswordLoginCommand(_command.EMail, _command.Password);
                var result = context.Service.ExecuteNowWithTimeout(validUserLoginQuery);

                result.Succeeded.ShouldBeTrue();
            });
        }

        [Fact]
        public void ThenADifferentPasswordShouldFailToLogin()
        {
            SessionScope((context) =>
            {
                var query = new UserEMailPasswordLoginCommand(_command.EMail, new Password("wrong password"));
                var result = context.Service.ExecuteNowWithTimeout(query);

                result.Succeeded.ShouldBeFalse();
            });
        }
    }


    public class WhenRegisteringAUserTwice : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private Result _result;

        public WhenRegisteringAUserTwice(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            var name = new Name(Guid.NewGuid().ToString("n"));
            var email = new EMail("eMail@future.now");
            var password = new Password("A-123assword");

            var command = new RegisterUserCommand(name, name, email, password, password);
            context.ShouldSucceed(command);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var name = new Name(Guid.NewGuid().ToString("n"));
            var email = new EMail("eMail@future.now");
            var password = new Password("A-123assword");

            var command = new RegisterUserCommand(name, name, email, password, password);
            _result = context.Service.ExecuteNowWithTimeout(command);
        }

        [Fact]
        public void ThenTheResultShouldFail()
        {
            _result.Succeeded.ShouldBeFalse();
        }

        [Fact]
        public void ThenTheResultShouldHaveErrors()
        {
            _result.Errors.Count().ShouldBe(1);
            _result.Errors.FirstOrDefault().Code.ShouldBe(ErrorCodes.DuplicateEmail);
        }
    }
}
