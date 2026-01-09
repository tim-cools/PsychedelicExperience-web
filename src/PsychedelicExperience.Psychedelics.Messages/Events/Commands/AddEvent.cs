using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class AddEvent : IRequest<Result>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public EventId EventId { get; }
        public EventPrivacy Privacy { get; }
        public EventType EventType { get; }
        public DateTime StartDateTime { get; }
        public DateTime? EndDateTime { get; }
        public EventLocation Location { get; }
        public Name Name { get; }
        public Description Description { get; }
        public Name[] Tags { get; }

        public AddEvent(UserId userId, OrganisationId organisationId, EventId eventId, EventType eventType, EventPrivacy privacy, Name name, Description description, DateTime startDateTime, DateTime? endDateTime, EventLocation location, Name[] tags)
        {
            UserId = userId;
            OrganisationId = organisationId;
            EventId = eventId;
            EventType = eventType;
            Privacy = privacy;
            Name = name;
            Description = description;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Location = location;
            Tags = tags ?? new Name[0];
        }
    }
}
