using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Marten.Schema;
using Marten.Services;
using MemBus;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.MessageBus;
using PsychedelicExperience.Common.Store;
using PsychedelicExperience.Common.StructureMap;
using StructureMap;

namespace PsychedelicExperience.Common
{
    public abstract class CommonRegistry : Registry
    {
        public static IList<Type> CustomTypeConverters { get; } = JsonConverterResolver.Resolve<CommonRegistry>();

        protected CommonRegistry()
        {
            Scan(options =>
            {
                options.TheCallingAssembly();
                options.IncludeNamespaceContainingType<ConnectionStringParser>();
                options.IncludeNamespaceContainingType<IMediator>();
                options.AssemblyContainingType<IMediator>();
                options.WithDefaultConventions();
                options.Convention<AllInterfacesConvention>();
            });

            RegisterMartenCommon();
            RegisterMartenSession();
            RegisterMartenEvents();
            RegisterServices();
            RegisterInmemoryBus();
            RegisterJsonConverters();
        }

        private void RegisterJsonConverters()
        {
            foreach (var customTypeConverter in CustomTypeConverters)
            {
                For(typeof(CustomTypeConverter)).Use(customTypeConverter);
            }
        }

        private void RegisterMartenEvents()
        {
            For<IEventStore>().Use<EventStore>();
        }

        private void RegisterMartenCommon()
        {
            For<ISerializer>().Use<JsonNetSerializer>();
            For<IDocumentCleaner>().Use<DocumentCleaner>();
        }

        private void RegisterMartenSession()
        {
            ForSingletonOf<IDocumentStore>()
                .Use("Create DocumentStore", context =>
                    DocumentStoreFactory.Create(context, ConfigureProjections));

            For<IQuerySession>()
                .Use("Create QuerySession", context => context.GetInstance<IDocumentStore>().QuerySession())
                .ContainerScoped();

            For<IDocumentSession>()
                .Use("Create DocumentSession", context => context.GetInstance<IDocumentStore>().DirtyTrackedSession())
                .ContainerScoped();
        }

        private void RegisterInmemoryBus()
        {
            ForSingletonOf<IBus>().Use("Create InMemoryBus", BusFactory.CreateInMemoryBus);
        }

        protected virtual void RegisterServices()
        {
        }

        protected abstract void ConfigureProjections(StoreOptions options, IEnumerable<IProjection> projections);
    }

    public class TestCommonRegistry : CommonRegistry
    {
        protected override void RegisterServices()
        {
            ForSingletonOf<IDaemonController>().Use<DummyDaemonController>();
        }

        protected override void ConfigureProjections(StoreOptions options, IEnumerable<IProjection> projections)
        {
            foreach (var projection in projections)
            {
                options.Events.InlineProjections.Add(projection);
            }
        }
    }

    public class ProductionCommonRegistry : CommonRegistry
    {
        protected override void RegisterServices()
        {
            ForSingletonOf<IDaemonController>().Use<DaemonController>();
            ForSingletonOf<IMailSender>().Use<MailSender>();
            ForSingletonOf<ISendInBlue>().Use<SendInBlue>();
        }

        protected override void ConfigureProjections(StoreOptions options, IEnumerable<IProjection> projections)
        {
            foreach (var projection in projections)
            {
                options.Events.AsyncProjections.Add(projection);
            }
        }
    }
}
