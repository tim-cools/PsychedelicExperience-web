using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using Marten;
using Marten.Events;

namespace PsychedelicExperience.Common.Aggregates
{
    public static class EventSourceExtensions
    {
        public static void ConfigureEventSourced<T>(this EventGraph configuration) where T : AggregateRoot, new()
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            configuration.AggregateFor<T>();
            configuration.InlineProjections.AggregateStreamsWith<T>();

            var type = typeof(T);
            var eventTypes = type.GetMethods()
                .Where(method => method.Name == "Apply")
                .SelectMany(method => method.GetParameters())
                .Select(parameter => parameter.ParameterType);

            eventTypes.Each(configuration.AddEventType);
        }

        public static T Load<T>(this IDocumentSession session, Guid id) where T : AggregateRoot, new()
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return session.Load<T>(id);
        }

        public static T Load<T>(this IDocumentSession session, Id id) where T : AggregateRoot, new()
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return session.Load<T>(id.Value);
        }

        public static IReadOnlyList<IEvent> LoadEvents(this IDocumentSession session, Guid id)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return session.Events.FetchStream(id);
        }

        public static IReadOnlyList<IEvent> LoadEvents(this IDocumentSession session, Id id)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return session.Events.FetchStream(id.Value);
        }

        public static Task<T> LoadAggregate<T>(this IDocumentSession session, Id id) where T : AggregateRoot, new()
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return session.Events.AggregateStreamAsync<T>(id.Value);
        }

        public static Task<T> LoadAggregate<T>(this IDocumentSession session, Guid id) where T : AggregateRoot, new()
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return session.Events.AggregateStreamAsync<T>(id);
        }

        public static IDocumentSession StoreChanges<T>(this IDocumentSession session, T aggregate) where T : AggregateRoot, new()
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));

            var events = aggregate.GetChanges();

            session.Events.Append(aggregate.Id, events);

            return session;
        }
    }
}