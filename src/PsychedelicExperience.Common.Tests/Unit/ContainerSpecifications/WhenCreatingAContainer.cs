using Microsoft.Extensions.Configuration;
using Shouldly;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Store;
using PsychedelicExperience.Common.Tests.Storage;
using StructureMap;
using Xunit;

namespace PsychedelicExperience.Common.Tests.Unit.ContainerSpecifications
{
    public class WhenCreatingAContainer
    {
        [Fact]
        public void ThenMessageHandlerShouldBeGettable()
        {
            var container = CreateContainer();

            container.GetInstance<ITestStoreDatabaseFactory>().ShouldNotBeNull<ITestStoreDatabaseFactory>();
        }

        [Fact]
        public void ThenTheMessagaDispatcherShouldBeGettable()
        {
            var container = CreateContainer();

            container.GetInstance<IMediator>().ShouldNotBeNull<IMediator>();
        }

        [Fact]
        public void ThenTheConnectionStringParserShouldBeGettable()
        {
            var container = CreateContainer();

            container.GetInstance<IConnectionStringParser>().ShouldNotBeNull<IConnectionStringParser>();
        }

        private static IContainer CreateContainer()
        {
            return TestContainerFactory.CreateContainer(configuration =>
            {
                configuration.For<IConfiguration>().Use<DummyConfiguration>();
            });
        }
    }
}
