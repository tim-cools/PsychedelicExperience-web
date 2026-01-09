using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    public class GetEventImageHandler : QueryHandler<GetEventImage, PhotoDetails>
    {
        public GetEventImageHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<PhotoDetails> Execute(GetEventImage query)
        {
            var @event = await Session.LoadAsync<Events.Event>(query.EventId);

            return @event?.Image != null 
                ? new PhotoDetails { Id = @event.Image.Id.Value, FileName = @event.Image.FileName } 
                : null;
        }
    }
}
