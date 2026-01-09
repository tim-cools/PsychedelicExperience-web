using System.Diagnostics;
using Baseline;
using Shouldly;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.RefreshTokens;
using StructureMap;
using Xunit;

namespace PsychedelicExperience.Membership.Tests.Unit.Infrastructure
{
    public class WhenCreatingAContainer : ServiceTestBase<IMediator>, IClassFixture<MembershipIntegrationTestFixture>
    {
        private IAsyncRequestHandler<DeleteRefreshTokenCommand, Result> _result;

        public WhenCreatingAContainer(MembershipIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            Document(context.Container);
            _result = context.Container.GetInstance<IAsyncRequestHandler<DeleteRefreshTokenCommand, Result>>();
        }

        private void Document(IContainer contextContainer)
        {
            contextContainer.Model.AllInstances.Each(i => Debug.WriteLine(i.PluginType.FullName));
        }

        [Fact]
        public void ThenMessageHandlerShouldBeGettable()
        {
            _result.ShouldNotBeNull();
        }
    }
}
