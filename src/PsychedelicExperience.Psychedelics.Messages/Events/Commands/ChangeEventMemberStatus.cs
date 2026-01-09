using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ChangeEventMemberStatus : IRequest<Result>
    {
        public EventId EventId { get; }
        public UserId MemberId { get; }
        public UserId UserId { get; }
        public EventMemberStatus Status { get; }

        public ChangeEventMemberStatus(EventId eventId, UserId memberId, UserId userId, EventMemberStatus status)
        {
            EventId = eventId;
            MemberId = memberId;
            UserId = userId;
            Status = status;
        }
    }
}