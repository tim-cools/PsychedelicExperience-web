using PsychedelicExperience.Common.Messages;
using Shouldly;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Unit.Infrastructure
{
    public class WhenCreatingAContainer : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private IAsyncRequestHandler<AddExperience, Result> _result;

        public WhenCreatingAContainer(PsychedelicsIntegrationTestFixture fixture) : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = context.Container.GetInstance<IAsyncRequestHandler<AddExperience, Result>>();
        }

        [Fact]
        public void ThenMessageHandlerShouldBeGettable()
        {
            _result.ShouldNotBeNull();
        }
    }
}
