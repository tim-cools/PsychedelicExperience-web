using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ClearEventImage : IRequest<Result>
    {
        public EventId EventId { get; }
        public UserId UserId { get; }

        public ClearEventImage(EventId eventId, UserId userId)
        {
            EventId = eventId;
            UserId = userId;
        }
    }
}