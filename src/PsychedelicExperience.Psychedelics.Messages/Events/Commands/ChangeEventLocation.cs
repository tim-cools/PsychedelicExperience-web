using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ChangeEventLocation : IRequest<Result>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }
        public EventLocation Location { get; }

        public ChangeEventLocation(UserId userId, EventId eventId, EventLocation location)
        {
            UserId = userId;
            EventId = eventId;
            Location = location;
            Location = location;
        }
    }
}