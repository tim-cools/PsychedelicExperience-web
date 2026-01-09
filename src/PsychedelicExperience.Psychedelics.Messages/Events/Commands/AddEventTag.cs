using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class AddEventTag : IRequest<Result>
    {
        public EventId EventId { get; }
        public UserId UserId { get; }
        public Name TagName { get; }

        public AddEventTag(EventId eventId, UserId userId, Name tagName)
        {
            EventId = eventId;
            UserId = userId;
            TagName = tagName;
        }
    }
}