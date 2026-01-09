using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Substances.Queries;

namespace PsychedelicExperience.Psychedelics.SubstanceView.Handlers
{
    public class MethodsQueryHandler : QueryHandler<MethodsQuery, Method[]>
    {
        private readonly Method[] _substances = 
        {
            new Method("Smoked"),
            new Method("Oral"), 
            new Method("Intravenous"),
        };

        public MethodsQueryHandler(IQuerySession session) : base(session)
        {
        }

        protected override Task<Method[]> Execute(MethodsQuery query)
        {
            var substances = query.QueryStringEmpty()
                ? DefaultMethods()
                : FilterMethods(query.QueryString);

            return Task.FromResult(substances);
        }

        private Method[] DefaultMethods()
        {
            return _substances;
        }

        private Method[] FilterMethods(string filter)
        {
            filter = filter.ToLowerInvariant();
            return _substances.Where(criteria => criteria.NormalizedName.Contains(filter)).ToArray();
        }
    }
}
