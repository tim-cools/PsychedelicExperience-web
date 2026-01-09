using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using StructureMap;
using PsychedelicExperience.Common.Tests.Storage;

namespace PsychedelicExperience.Common.Tests
{
    public abstract class IntegrationTestFixture : IDisposable
    {
        private Action _onDispose;
        private static bool _databaseCreated;
        private readonly IContainer _container;

        protected IntegrationTestFixture()
        {
            LoggingInitializer.Initialize();

            var configuration = InitializeConfiguration();
            _container = InitializeContainer(configuration);

            InitializeDatabase();
        }

        public IContainer OpenContainerScope()
        {
            return _container.CreateChildContainer();
        }

        public void OnDispose(Action action)
        {
            _onDispose = action;
        }

        private void InitializeDatabase()
        {
            if (!_databaseCreated)
            {
                var factory = _container.GetInstance<ITestStoreDatabaseFactory>();
                factory.CreateCleanStoreDatabase();
                _databaseCreated = true;
            }
        }

        private IConfigurationRoot InitializeConfiguration()
        {
            var environment = System.Environment.GetEnvironmentVariable("Hosting:Environment") ?? "local";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.tests.json")
                .AddJsonFile($"appsettings.tests.{environment}.json")
                ;

            return builder.Build();
        }

        private IContainer InitializeContainer(IConfiguration configuration)
        {
            var container = new Container(config =>
            {
                InitializeContainer(config);

                config.AddRegistry<TestCommonRegistry>();
                config.AddRegistry<TestRegistry>();
                config.For<IConfiguration>().Use(configuration);
            });

            return container;
        }

        protected abstract void InitializeContainer(ConfigurationExpression configuration);

        public void Dispose()
        {
            Console.WriteLine("Dispose test");

            _onDispose?.Invoke();

            _container.Dispose();
        }
    }
}