using System;
using Shouldly;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Clients;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Integration.Clients
{
    public class WhenValidatingClientRedirectUriGivenClientIsNotKnown : ServiceTestBase<IMediator>,
        IClassFixture<MembershipIntegrationTestFixture>
    {
        private Result _result;

        public WhenValidatingClientRedirectUriGivenClientIsNotKnown(MembershipIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            var clientId = Guid.NewGuid().ToString("n");
            var redirectUri = Guid.NewGuid().ToString("n");
            var validator = new ClientRedirectUriValidator(clientId, redirectUri);

            _result = context.Service.ExecuteNowWithTimeout(validator);
        }

        [Fact]
        public void ThenClientShouldBeInvalid()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeFalse();
        }
    }
}