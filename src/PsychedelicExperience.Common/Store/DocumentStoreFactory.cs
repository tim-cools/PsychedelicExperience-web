using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events.Projections;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using StatsdClient;
using StructureMap;

namespace PsychedelicExperience.Common.Store
{
    internal class DocumentStoreFactory
    {
        public static IDocumentStore Create(IContext context, Action<StoreOptions, IEnumerable<IProjection>> configureProjections)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (configureProjections == null) throw new ArgumentNullException(nameof(configureProjections));

            var connectionString = GetConnectionString(context);

            var logger = context.GetInstance<ILogger<DocumentStoreFactory>>();

            logger.LogDebug($"Create DocumentStore: {connectionString}");

            var documentStores = context.GetAllInstances<MartenRegistry>();
            var converters = context.GetAllInstances<CustomTypeConverter>()
                .ToDictionary(converter => converter.Type, converter => (JsonConverter)converter);

            var projections = context.GetAllInstances<IProjection>();
            var configuration = context.GetInstance<IConfiguration>();
            var martenLogger = context.GetInstance<LoggerAdapter>();

            var store = DocumentStore.For(options =>
            {
                options.Connection(connectionString);
                options.Serializer(new JsonNetWithPrivateSupportSerializer(converters));
                options.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                options.Logger(martenLogger);
                options.DatabaseSchemaName = "psychedelics";
                options.Events.DatabaseSchemaName = "psychedelics_events";
                options.Listeners.Add(new PublishStatsDEvents(configuration));

                InitializeEvents(documentStores, options);
                InitializeProjections(configureProjections, configuration, options, projections, logger);
            });

            store.Schema.ApplyAllConfiguredChangesToDatabase();

            return store;
        }

        private static void InitializeEvents(IEnumerable<MartenRegistry> documentStores, StoreOptions options)
        {
            foreach (var documentStore in documentStores)
            {
                options.Schema.Include(documentStore);

                var initEvents = documentStore as IInitializeEvents;
                initEvents?.Initialize(options.Events);
            }
        }

        private static void InitializeProjections(Action<StoreOptions, IEnumerable<IProjection>> configureProjections, IConfiguration configuration,
            StoreOptions options, IEnumerable<IProjection> projections, ILogger<DocumentStoreFactory> logger)
        {
            if (configuration.RunProjections())
            {
                configureProjections(options, projections);
            }
            else
            {
                logger.LogWarningMethod($"Running of document projections is disabled, skipping projections initialization.");
            }
        }

        private static string GetConnectionString(IContext context)
        {
            var connectionString = context.GetInstance<ConnectionStringParser>().GetString();
            if (connectionString == null)
            {
                throw new InvalidOperationException("Connection string not found!");
            }
            return connectionString;
        }
    }

    internal class PublishStatsDEvents : IDocumentSessionListener
    {
        private readonly ConcurrentDictionary<Assembly, string> _assemblyNamespace = new ConcurrentDictionary<Assembly, string>();
        private readonly ConcurrentDictionary<Type, string> _typeKeys = new ConcurrentDictionary<Type, string>();
        private readonly string[] _tags;

        public PublishStatsDEvents(IConfiguration configuration)
        {
            _tags = new[]
            {
                "env:" + configuration.Environment(),
                "application:web"
            };
        }

        public void BeforeSaveChanges(IDocumentSession session)
        {
        }

        public Task BeforeSaveChangesAsync(IDocumentSession session, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public void AfterCommit(IDocumentSession session, IChangeSet commit)
        {
            var events = commit.GetEvents();
            foreach (var @event in events)
            {
                var type = @event.Data.GetType();
                var key = _typeKeys.GetOrAdd(type, GetKey);

                DogStatsd.Increment(key, tags: _tags);
            }
        }

        private string GetKey(Type type)
        {
            var @namespace = _assemblyNamespace.GetOrAdd(type.GetTypeInfo().Assembly, GetNamespace);
            var builder = new StringBuilder(@namespace);
            var typeName = type.Name;
            var first = true;

            foreach (var character in typeName)
            {
                if (char.IsUpper(character) && first)
                {
                    builder.Append(".");
                    builder.Append(char.ToLower(character));
                    first = false;
                }
                else
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
        }

        private string GetNamespace(Assembly assembly)
        {
            return assembly.GetName().Name
                .ToLowerInvariant()
                .Substring("psychedelicexperience.".Length);
        }

        public Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
        {
            AfterCommit(session, commit);

            return Task.CompletedTask;
        }

        public void DocumentLoaded(object id, object document)
        {
        }

        public void DocumentAddedForStorage(object id, object document)
        {
        }
    }

    internal class LoggerAdapter : IMartenLogger
    {
        public const string Name = "Marten";

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public LoggerAdapter(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger(Name);
        }

        public IMartenSessionLogger StartSession(IQuerySession session)
        {
            return new SessionLoggerAdapter(_loggerFactory);
        }

        public void SchemaChange(string sql)
        {
            _logger.LogInformationMethod(nameof(SchemaChange), new { sql });
        }
    }

    internal class SessionLoggerAdapter : IMartenSessionLogger
    {
        public const string Name = "Marten.Session";

        private readonly ILogger _logger;

        public SessionLoggerAdapter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(Name);
            _logger.LogDebugMethod("SessionStarted");
        }

        public void LogSuccess(NpgsqlCommand command)
        {
            _logger.LogDebugMethod("Success", new { command.CommandText });
        }

        public void LogFailure(NpgsqlCommand command, Exception ex)
        {
            _logger.LogWarningMethod("Failure", new { command.CommandText });
        }

        public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
        {
            _logger.LogDebugMethod("Persisted", new
            {
                updates = commit.Updated.Count(),
                inserts = commit.Inserted.Count(),
                deletes = commit.Deleted.Count()
            });
        }
    }
}