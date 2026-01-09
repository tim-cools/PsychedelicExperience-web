using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Shouldly;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.RefreshTokens;
using StructureMap;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Integration.RefreshTokens
{
    public class WhenQueryingRefreshTokens : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private IEnumerable<RefreshToken> _result;

        public WhenQueryingRefreshTokens(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            var command = new CreateRefreshTokenCommand(
              Guid.NewGuid().ToString(),
              Guid.NewGuid().ToString(),
              Guid.NewGuid().ToString(),
              Guid.NewGuid().ToString(),
              Guid.NewGuid().ToString(),
              DateTimeOffset.Now,
              DateTimeOffset.Now);

            var result = context.Service.ExecuteNowWithTimeout(command);
            result.Succeeded.ShouldBeTrue(result.ToString());
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new RefreshTokensQuery();
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheRefreshTokensShouldBeReturned()
        {
            _result.ShouldNotBeNull();
        }

        [Fact]
        public void ThenAsLeastOnRefreshTokenShouldBeReturned()
        {
            _result.Count().ShouldBeGreaterThanOrEqualTo(1);
        }
    }
}