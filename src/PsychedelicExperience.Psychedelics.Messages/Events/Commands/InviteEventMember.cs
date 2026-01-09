using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class InviteEventMember : IRequest<Result>
    {
        public EventId EventId { get; }
        public UserId UserId { get; }
        public UserId MemberId { get; }

        public InviteEventMember(EventId eventId, UserId userId, UserId memberId)
        {
            EventId = eventId;
            UserId = userId;
            MemberId = memberId;
        }
    }
}