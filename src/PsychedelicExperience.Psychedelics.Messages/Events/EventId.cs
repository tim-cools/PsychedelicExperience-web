using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Events
{
    public class EventId : Id
    {
        public EventId(Guid value) : base(value)
        {
        }

        public static EventId New()
        {
            return new EventId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator EventId(Guid id)
        {
            return new EventId(id);
        }
    }
}