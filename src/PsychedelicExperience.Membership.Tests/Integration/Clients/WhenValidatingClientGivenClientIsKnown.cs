using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Clients;
using Shouldly;
using Xunit;
using Client = PsychedelicExperience.Membership.Clients.Domain.Client;

namespace PsychedelicExperience.Membership.Tests.Integration.Clients
{
    public class WhenValidatingClientGivenClientIsKnown : ServiceTestBase<IMediator>,
        IClassFixture<MembershipIntegrationTestFixture>
    {
        private ValidateClientResult _result;
        private Client _client;

        public WhenValidatingClientGivenClientIsKnown(MembershipIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _client = ClientFactory.Create();

            context.Session.Store(_client);
            context.Session.SaveChanges();
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new ClientValidator(_client.Key, _client.Secret);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenClientShouldBeValid()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeTrue(_result.ToString);
        }
    }
}