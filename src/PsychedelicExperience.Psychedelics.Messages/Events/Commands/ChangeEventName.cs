using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ChangeEventName : IRequest<Result>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }
        public Name Name { get; }

        public ChangeEventName(UserId userId, EventId eventId, Name name)
        {
            UserId = userId;
            EventId = eventId;
            Name = name;
        }
    }
}