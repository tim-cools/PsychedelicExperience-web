using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Substances.Queries;

namespace PsychedelicExperience.Psychedelics.SubstanceView.Handlers
{
    public class UnitsQueryHandler : QueryHandler<UnitsQuery, Unit[]>
    {
        private readonly Unit[] _units = 
        {
            new Unit("gr"),
            new Unit("mg"),
            new Unit("µg"),
            new Unit("piece"),
            new Unit("blotter"),
            new Unit("drops"),
            new Unit("cups"),
            new Unit("tables"),
            new Unit("microdot"),
            new Unit("hits"),
            new Unit("hits")
        };

        public UnitsQueryHandler(IQuerySession session) : base(session)
        {
        }

        protected override Task<Unit[]> Execute(UnitsQuery query)
        {
            var units = query.QueryStringEmpty()
                ? DefaultUnits()
                : FilterUnits(query.QueryString);

            return Task.FromResult(units);
        }

        private Unit[] DefaultUnits()
        {
            return _units;
        }

        private Unit[] FilterUnits(string filter)
        {
            filter = filter.ToLowerInvariant();
            return _units.Where(criteria => criteria.NormalizedName.Contains(filter)).ToArray();
        }
    }
}
