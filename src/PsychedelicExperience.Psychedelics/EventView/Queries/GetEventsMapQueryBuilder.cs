using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    public class GetEventsMapQueryBuilder
    {
        private readonly ExpressionStarter<Event> _where;
        private readonly IQuerySession _session;

        public GetEventsMapQueryBuilder(ExpressionStarter<Event> criteria, IQuerySession session)
        {
            _session = session;
            _where = criteria;
        }

        public async Task<EventMapPoint[]> Execute()
        {
            var values = await _session.Query<Event>()
                .Where(_where).OrderBy(organisation => organisation.StartDateTime)
                .Select(organisation => new
                {
                    id = organisation.Id,
                    name = organisation.Name,
                    address = organisation.Address
                })
                .ToListAsync();

            return values
                .Where(value => value.address != null)
                .Select(value => new EventMapPoint
                {
                    Id = value.id,
                    Name = value.name,
                    Position = value.address.Position
                }).ToArray();
        }
    }
}