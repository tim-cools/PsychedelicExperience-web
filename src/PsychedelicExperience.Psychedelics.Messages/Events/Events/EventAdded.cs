using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventAdded : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public OrganisationId OrganisationId { get; set; }
        public EventPrivacy Privacy { get; set; }
        public EventType EventType { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public EventLocation Location { get; set; }
        public Name Name { get; set; }
        public Description Description { get; set; }
        public Name[] Tags { get; set; }
    }
}
