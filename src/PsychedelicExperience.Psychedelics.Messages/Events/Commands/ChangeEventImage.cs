using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ChangeEventImage : IRequest<Result>
    {
        public EventId EventId { get; }
        public UserId UserId { get; }
        public Image Image { get; }

        public ChangeEventImage(EventId eventId, UserId userId, Image image)
        {
            EventId = eventId;
            UserId = userId;
            Image = image;
        }
    }
}