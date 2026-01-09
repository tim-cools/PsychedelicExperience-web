using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class RemoveEvent : IRequest<Result>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }

        public RemoveEvent(UserId userId, EventId eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
    }
}