using System;
using System.Reflection;

namespace PsychedelicExperience.Common.Aggregates
{
    public static class AggregateInvoker
    {
        public static void ApplyEvent(object aggregate, object @event)
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            //todo we should generate some code to invoke the methed and cache it here
            var aggregateType = aggregate.GetType();
            var eventType = @event.GetType();

            var method = aggregateType.GetMethod("Apply", new[] { eventType });
            if (method == null)
            {
                throw new InvalidOperationException($"No 'Apply' method found for aggregate '{aggregateType}' and event '{eventType}");
            }
            method.Invoke(aggregate, new[] { @event });
        }

        public static void HandleCommand<T>(object aggregate, T user, object command) where T : class 
        {
            if (aggregate == null) throw new ArgumentNullException(nameof(aggregate));
            if (command == null) throw new ArgumentNullException(nameof(command));

            //todo we should generate some code to invoke the mothed and cache it here
            var aggregateType = aggregate.GetType();
            var commandType = command.GetType();

            var method = aggregateType.GetMethod("Handle", new[] { typeof(T), commandType });
            if (method == null)
            {
                throw new InvalidOperationException($"No 'Handle' method found for aggregate '{aggregateType}' and command '{commandType}");
            }
            method.Invoke(aggregate, new[] { user, command });
        }
    }
}