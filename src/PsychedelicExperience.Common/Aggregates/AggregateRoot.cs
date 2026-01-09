using System;
using System.Collections.Generic;
using Marten;
using System.Linq;

namespace PsychedelicExperience.Common.Aggregates
{
    public class AggregateRoot : IIdentifyable
    {
        private readonly IList<object> _events = new List<object>();

        public Guid Id { get; protected set; }

        protected void Publish<TEvent>(TEvent @event) where TEvent : Event
        {
            @event.EventTimestamp = DateTime.Now;

            _events.Add(@event);

            AggregateInvoker.ApplyEvent(this, @event);
        }

        public object[] GetChanges()
        {
            return _events.ToArray();
        }
    }
}
