using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ChangeEventPrivacy : IRequest<Result>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }
        public EventPrivacy Privacy { get; }

        public ChangeEventPrivacy(UserId userId, EventId eventId, EventPrivacy privacy)
        {
            UserId = userId;
            EventId = eventId;
            Privacy = privacy;
        }
    }
}