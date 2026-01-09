using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Marten.Events.Projections.Async;
using Marten.Storage;

namespace PsychedelicExperience.Common.Views
{
    public abstract class EventProjection : IDocumentProjection
    {
        private class StreamEvent
        {
            public EventStream Stream { get; }
            public IEvent Event { get; }

            public StreamEvent(EventStream stream, IEvent @event)
            {
                Stream = stream;
                Event = @event;
            }
        }

        private readonly IDictionary<Type, Action<IDocumentSession, StreamEvent>> _handlers = new ConcurrentDictionary<Type, Action<IDocumentSession, StreamEvent>>();
        private readonly IDictionary<Type, Func<IDocumentSession, StreamEvent, Task>> _asyncHandlers = new ConcurrentDictionary<Type, Func<IDocumentSession, StreamEvent, Task>>();

        public Type[] Consumes => _handlers.Keys.Union(_asyncHandlers.Keys).ToArray();
        public Type Produces => ViewType;
        public abstract Type ViewType { get; }
        public AsyncOptions AsyncOptions { get; } = new AsyncOptions();

        protected EventProjection Event<T>(Action<IDocumentSession, Guid, T> handler) where T : PsychedelicExperience.Common.Aggregates.Event
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _handlers.Add(typeof(T), (session, streamEvent) => handler(session, streamEvent.Stream.Id, streamEvent.Event.Data as T));

            return this;
        }

        protected EventProjection EventAsync<T>(Func<IDocumentSession, Guid, T, Task> handler) where T : PsychedelicExperience.Common.Aggregates.Event
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _asyncHandlers.Add(typeof(T), (session, streamEvent) => handler(session, streamEvent.Stream.Id, streamEvent.Event.Data as T));

            return this;
        }

        public void Apply(IDocumentSession session, EventPage page)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            var events = GetEvents(page);

            foreach (var @event in events)
            {
                Action<IDocumentSession, StreamEvent> handler;
                if (_handlers.TryGetValue(@event.Event.Data.GetType(), out handler))
                {
                    handler(session, @event);
                }
            }
        }

        public async Task ApplyAsync(IDocumentSession session, EventPage page, CancellationToken token)
        {
            var events = GetEvents(page);

            foreach (var @event in events)
            {
                if (_asyncHandlers.TryGetValue(@event.Event.Data.GetType(), out var handler))
                {
                    await handler(session, @event);
                }
            }
        }
        
        public void EnsureStorageExists(ITenant tenant)
        {
            tenant.EnsureStorageExists(Produces);
        }
        
        private static IEnumerable<StreamEvent> GetEvents(EventPage page)
        {
            return page.Streams.SelectMany(stream => stream.Events.Select(@event => new StreamEvent(stream, @event)));
        }
    }
}