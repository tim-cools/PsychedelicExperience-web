using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class JoinEventMember : IRequest<Result>
    {
        public EventId EventId { get; }
        public UserId UserId { get; }
        public EventMemberStatus Status { get; }

        public JoinEventMember(EventId eventId, UserId userId, EventMemberStatus status)
        {
            EventId = eventId;
            UserId = userId;
            Status = status;
        }
    }
}