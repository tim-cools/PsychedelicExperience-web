using System;
using System.Collections.Generic;
using System.Linq;
using Marten.Events;
using PsychedelicExperience.Common.Aggregates;
using Shouldly;

namespace PsychedelicExperience.Common.Tests.Messages
{
    public static class EventsExtensions
    {
        public static T LastEventShouldBeOfType<T>(this IEnumerable<IEvent> events) where T : Event
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            var last = events.Last();
            last.ShouldNotBeNull($"LastEventShouldBeOfType: {typeof(T).FullName} is null");
            last.Data.ShouldNotBeNull($"LastEventShouldBeOfType: {typeof(T).FullName} DATA is null");
            last.Data.ShouldBeOfType<T>();
            return (T) last.Data;
        }

        public static T EventShouldBeOfType<T>(this IEvent[] events, int index)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            var elementAt = events.ElementAt(index);
            elementAt.ShouldNotBeNull($"EventShouldBeOfType: {typeof(T).FullName} at index {index} is null");

            return elementAt.Data.ShouldBeOfType<T>();
        }
    }
}
