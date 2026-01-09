using System;
using System.Linq;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Messages.Users;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Integration.UserProfiles
{
    public class WhenCreatingAUserProfile : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private readonly UserId _userId =  UserId.New();
        private EMail _email;
        private Result _result;
        private Name _userName;
        private Name _fullName;

        public WhenCreatingAUserProfile(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            var name = CombGuidIdGeneration.NewGuid().ToString("N");
            _fullName = new Name("f" + name);
            _userName = new Name(name);
            _email = new EMail(_userName + "@fbi-cia-nsa.us");
            var password = new Password("A-" + Guid.NewGuid().ToString("n"));

            var command = new RegisterUserCommand(_fullName, _userName, _email, password, password);
            _result = context.Service.ExecuteNowWithTimeout(command);
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
        public void ThenAUserProfileShouldBeStoredWithUserName()
        {
            SessionScope(context =>
            {
                var query = new UserProfileByEMailQuery(_userId, _email);
                var profile = context.Service.ExecuteNowWithTimeout(query);

                profile.ShouldNotBeNull();
                profile.DisplayName.ShouldBe(_userName.ToString());
                profile.FullName.ShouldBe(_fullName.ToString());
            });
        }
    }
}
