using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class GetEvent : IRequest<EventDetails>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }

        public GetEvent(UserId userId, EventId eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
    }
}
