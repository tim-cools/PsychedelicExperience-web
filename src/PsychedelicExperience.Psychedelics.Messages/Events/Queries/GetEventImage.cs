using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class GetEventImage : IRequest<PhotoDetails>
    {
        public EventId EventId { get; }

        public GetEventImage(EventId eventId)
        {
            EventId = eventId;
        }
    }
}