using System;
using Shouldly;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Clients;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Integration.Clients
{
    public class WhenValidatingClientGivenClientIsNotKnown : ServiceTestBase<IMediator>,
        IClassFixture<MembershipIntegrationTestFixture>
    {
        private ValidateClientResult _result;

        public WhenValidatingClientGivenClientIsNotKnown(MembershipIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            var clientId = Guid.NewGuid().ToString("n");
            var clientSecret = Guid.NewGuid().ToString("n");

            var query = new ClientValidator(clientId, clientSecret);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenClientShouldBeInvalid()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeFalse();
        }
    }
}