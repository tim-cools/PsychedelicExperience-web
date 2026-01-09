using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Clients;
using Shouldly;
using Xunit;
using Client = PsychedelicExperience.Membership.Clients.Domain.Client;

namespace PsychedelicExperience.Membership.Tests.Integration.Clients
{
    public class WhenValidatingClientRedirectUriGivenRedirectUriIsInvalid : ServiceTestBase<IMediator>,
        IClassFixture<MembershipIntegrationTestFixture>
    {
        private Client _client;
        private Result _result;

        public WhenValidatingClientRedirectUriGivenRedirectUriIsInvalid(MembershipIntegrationTestFixture fixture)
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
            var validator = new ClientRedirectUriValidator(_client.Key, _client.RedirectUri);
            _result = context.Service.ExecuteNowWithTimeout(validator);
        }

        [Fact]
        public void ThenClientShouldBeValid()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeTrue(_result.ToString);
        }
    }
}