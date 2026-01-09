using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ReportEvent : IRequest<Result>
    {
        public EventId EventId { get; }
        public UserId UserId { get; }
        public Description Reason { get; }

        public ReportEvent(EventId eventId, UserId userId, Description reason)
        {
            EventId = eventId;
            UserId = userId;
            Reason = reason;
        }
    }
}