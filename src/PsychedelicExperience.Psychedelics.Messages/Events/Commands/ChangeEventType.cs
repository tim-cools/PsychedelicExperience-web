using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ChangeEventType : IRequest<Result>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }
        public EventType EventType { get; }

        public ChangeEventType(UserId userId, EventId eventId, EventType eventType)
        {
            UserId = userId;
            EventId = eventId;
            EventType = eventType;
        }
    }
}