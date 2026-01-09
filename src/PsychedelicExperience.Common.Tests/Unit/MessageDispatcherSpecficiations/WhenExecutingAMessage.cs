using Shouldly;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests.Unit.ContainerSpecifications;
using Xunit;
using TestMessage = PsychedelicExperience.Common.Tests.Unit.ContainerSpecifications.TestMessage;

namespace PsychedelicExperience.Common.Tests.Unit.MessageDispatcherSpecficiations
{
    public class WhenExecutingAMessage
    {
        [Fact]
        public async void ThenTheMessageHandlerShouldHaveItsOwnSession()
        {
            DummeDocumentSession session = null;
            var container = TestContainerFactory.CreateContainer(config =>
            {
                config.For<IDummySession>().Use("TestSession", context => session = new DummeDocumentSession()).ContainerScoped();
                config.For<IAsyncRequestHandler<TestMessage, TestMessage>>().Use<DummyMessageHandler>();
            });

            var mediator = container.GetInstance<IMediator>();
            mediator.ShouldNotBeNull();

            await mediator.Send(new TestMessage());

            session.ShouldNotBeNull();
            session.Disposed.ShouldBeTrue();
        }
    }
}
