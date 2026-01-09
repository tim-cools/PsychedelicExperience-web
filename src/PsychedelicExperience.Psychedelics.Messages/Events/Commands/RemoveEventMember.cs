using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class RemoveEventMember : IRequest<Result>
    {
        public EventId EventId { get; }
        public UserId MemberId { get; }
        public UserId UserId { get; }

        public RemoveEventMember(EventId eventId, UserId memberId, UserId userId)
        {
            EventId = eventId;
            MemberId = memberId;
            UserId = userId;
        }
    }
}