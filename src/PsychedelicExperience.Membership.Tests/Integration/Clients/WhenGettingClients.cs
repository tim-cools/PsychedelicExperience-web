using System;
using System.Linq;
using Shouldly;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Clients.Domain;
using PsychedelicExperience.Membership.Messages.Clients;
using Xunit;
using Xunit.Abstractions;
using Client = PsychedelicExperience.Membership.Clients.Domain.Client;

namespace PsychedelicExperience.Membership.Tests.Integration.Clients
{
    public class WhenGettingClients : ServiceTestBase<IMediator>,
        IClassFixture<MembershipIntegrationTestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private Messages.Clients.Client[] _result;
        private Client _client;

        public WhenGettingClients(MembershipIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            CreateClient(context);
        }

        private void CreateClient(TestContext<IMediator> context)
        {
            _client = new Client
            {
                Id = Guid.NewGuid(),
                Key = Guid.NewGuid().ToString("n"),
                Name = Guid.NewGuid().ToString("n"),
                ApplicationType = ApplicationTypes.JavaScript,
                Active = true
            };

            context.Session.Store(_client);
            context.Session.SaveChanges();
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetClients();
            _result = context.Service.ExecuteNowWithTimeout(query, _testOutputHelper);
        }

        [Fact]
        public void ThenClientsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            var newClient = _result.FirstOrDefault(client => client.Id == _client.Id);
            newClient.ShouldNotBeNull();
            newClient.Name.ShouldBe(_client.Name);
        }
    }
}