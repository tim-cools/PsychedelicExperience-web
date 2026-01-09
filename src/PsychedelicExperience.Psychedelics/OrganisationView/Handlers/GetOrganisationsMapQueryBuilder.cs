using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationsMapQueryBuilder
    {
        private ExpressionStarter<Organisation> _where;
        private readonly IQuerySession _session;

        public GetOrganisationsMapQueryBuilder(IQuerySession session)
        {
            _session = session;
            _where = PredicateBuilder.New<Organisation>();
        }

        public GetOrganisationsMapQueryBuilder FilterCountry(string country)
        {
            if (!string.IsNullOrWhiteSpace(country))
            {
                country = country.NormalizeForSearch();

                _where = _where.And(experience => experience.Address.Country == country);
            }
            return this;
        }

        public GetOrganisationsMapQueryBuilder WithPrivacy(User user)
        {
            if (user == null || !user.IsAdministrator())
            {
                _where = _where.And(experience => !experience.Removed);
            }
            else
            {
                _where = _where.And(experience => experience.Removed || !experience.Removed);
            }
            return this;
        }

        public GetOrganisationsMapQueryBuilder FilterTags(string[] tags)
        {
            if (tags == null || tags.Length == 0) return this;

            foreach (var tag in tags)
            {
                var value = tag.NormalizeForSearch();
                _where = _where.And(organisation => organisation.TagsNormalized.Contains(value));
            }
            return this;
        }

        public async Task<OrganisationMapPoint[]> Execute()
        {
            var values = await _session.Query<Organisation>()
                .Where(_where)
                .OrderBy(organisation => organisation.Created)
                .Select(organisation => new
                {
                    id = organisation.Id,
                    address = organisation.Address
                })
                .ToListAsync();

            return values
                .Where(value => value.address != null)
                .Select(value => new OrganisationMapPoint
            {
                Id = value.id,
                Position = value.address.Position
            }).ToArray();
        }
    }
}