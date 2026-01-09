using System.Linq;
using Shouldly;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Clients.Domain;
using PsychedelicExperience.Membership.Messages.Infrastructure;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Integration.Infrastructure
{
    public class WhenInitializingTheDatabase : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private Result _result;
        private InitializeDatabaseCommand _command;

        public WhenInitializingTheDatabase(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _command = new InitializeDatabaseCommand();
            _result = context.Service.ExecuteNowWithTimeout(_command);
        }

        [Fact]
        public void ThenTheRefreshTokensShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeTrue();
        }

        [Fact]
        public void ThenClientShouldBeCreated()
        {
            SessionScope(context =>
            {
                context.Session.Query<Client>().Count().ShouldBeGreaterThan(0);
            });
        }

        [Fact]
        public void ThenUsersShouldBeCreated()
        {
            SessionScope(context =>
            {
                context.Session.Query<Users.Domain.User>().Count().ShouldBeGreaterThan(0);
            });
        }
    }
}