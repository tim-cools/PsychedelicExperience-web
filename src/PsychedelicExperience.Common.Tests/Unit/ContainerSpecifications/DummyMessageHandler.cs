using System.Threading.Tasks;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Common.Tests.Unit.ContainerSpecifications
{
    public class DummyMessageHandler : IAsyncRequestHandler<TestMessage, TestMessage>
    {
        public DummyMessageHandler(IDummySession session)
        {
            
        }

        public Task<TestMessage> Handle(TestMessage query)
        {
            return Task.FromResult(new TestMessage());
        }
    }
}