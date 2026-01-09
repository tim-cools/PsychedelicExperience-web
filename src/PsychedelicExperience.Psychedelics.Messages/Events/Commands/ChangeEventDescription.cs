using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ChangeEventDescription : IRequest<Result>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }
        public Description Description { get; }

        public ChangeEventDescription(UserId userId, EventId eventId, Description description)
        {
            UserId = userId;
            EventId = eventId;
            Description = description;
        }
    }
}