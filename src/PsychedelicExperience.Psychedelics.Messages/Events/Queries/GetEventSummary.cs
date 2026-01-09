using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class GetEventSummary : IRequest<EventSummary>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }

        public GetEventSummary(UserId userId, EventId eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
    }
}