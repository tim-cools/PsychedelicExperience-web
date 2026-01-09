using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventStartDateTimeChanged : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public DateTime DateTime { get; set; }
    }
    public class EventEndDateTimeChanged : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public DateTime? DateTime { get; set; }
    }
}