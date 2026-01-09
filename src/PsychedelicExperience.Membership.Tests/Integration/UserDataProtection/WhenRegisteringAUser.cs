using System.Text;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Security;
using Shouldly;
using Xunit;


namespace PsychedelicExperience.Membership.Tests.Integration.UserDataProtection
{
    public class WhenEncrypting : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private readonly string _data = "0123456488679798406sq5d4qsd654sdq604qsd-8qsd-sqd98qsd-+7zea9687DZ9+382D39A+1-8-279";
        private readonly UserId _userId =  UserId.New();
        private EncryptedString _secure;
        private string _result;

        public WhenEncrypting(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var dataProtector = context.Container.GetInstance<IUserDataProtector>();
            _secure = dataProtector.Encrypt(_userId, _data);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var dataProtector = context.Container.GetInstance<IUserDataProtector>();
            _result = dataProtector.Decrypt(_userId, _secure);
        }

        [Fact]
        public void ThenTheResultShouldBeTheOriginal()
        {
            _result.ShouldBe(_data);
        }

        [Fact]
        public void ThenTheSecuredStringShouldNotBeTheOriginal()
        {
            _secure.Value.ShouldNotBe(_data);
        }
    }
}
