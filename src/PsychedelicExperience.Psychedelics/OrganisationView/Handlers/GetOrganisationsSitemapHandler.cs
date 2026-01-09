using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Addresses;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationsSitemapHandler : QueryHandler<GetOrganisationsSitemap, OrganisationsSitemapResult>
    {
        private readonly ICountryMapper _countryMapper;

        public GetOrganisationsSitemapHandler(IQuerySession session, ICountryMapper countryMapper) : base(session)
        {
            _countryMapper = countryMapper;
        }

        protected override async Task<OrganisationsSitemapResult> Execute(GetOrganisationsSitemap getOrganisationsQuery)
        {
            var organisations = await Session.Query<Organisation>()
                .Where(organisation => !organisation.Removed)
                .Select(organisation => new OrganisationsSitemapEntry
                {
                    Id = organisation.Id,
                    Name = organisation.Name,
                    Country = organisation.Country,
                    Tags = organisation.Tags,
                    Types = organisation.Types
                })
                .ToListAsync();

            var typesTagsCountries = organisations
                .Where(organisation => organisation.Country != null)
                .GroupBy(organisation => organisation.Country)
                .SelectMany(countryGroup => countryGroup
                    .Where(organisation => organisation.Types != null)
                    .SelectMany(organisation => organisation.Types)
                    .Where(type => type != null)
                    .Distinct()
                    .SelectMany(type => countryGroup
                        .Where(organisation => organisation.Types?.Contains(type) == true)
                        .Where(organisation => organisation.Tags != null)
                        .SelectMany(organisation => organisation.Tags)
                        .Where(tag => tag != null)
                        .Distinct()
                        .Select(tag => new OrganisationsTypeTagCountry
                        {
                            Country = _countryMapper.GetCountry(countryGroup.Key).NormalizeForUrl(),
                            Type = type.NormalizeForUrl(),
                            Tag = tag.NormalizeForUrl()
                        })))
                .ToArray();

            var typesTags = organisations
                .Where(organisation => organisation.Types != null)
                .SelectMany(organisation => organisation.Types)
                .Where(type => type != null)
                .Distinct()
                .SelectMany(type => organisations
                    .Where(organisation => organisation.Types?.Contains(type) == true)
                    .SelectMany(organisation => organisation.Tags)
                    .Where(tag => tag != null)
                    .Distinct()
                    .Select(tag => new OrganisationsTypeTag
                    {
                        Type = type.NormalizeForUrl(),
                        Tag = tag.NormalizeForUrl()
                    }))
                .ToArray();

            var typesCountries = organisations
                .Where(organisation => organisation.Country != null)
                .GroupBy(organisation => organisation.Country)
                .SelectMany(group => group
                    .Where(organisation => organisation.Types != null)
                    .SelectMany(organisation => organisation.Types)
                    .Distinct()
                    .Select(type => new OrganisationsTypeCountry
                    {
                        Country = _countryMapper.GetCountry(group.Key).NormalizeForUrl(),
                        Type = type.NormalizeForUrl()
                    }))
                .ToArray();

            var types = organisations
                .Where(organisation => organisation.Types != null)
                .SelectMany(organisation => organisation.Types)
                .Select(type => type.NormalizeForUrl())
                .Distinct()
                .ToArray();

            return new OrganisationsSitemapResult
            {
                Types = types,
                TypesCountries = typesCountries,
                TypesTags = typesTags,
                TypesTagsCountries = typesTagsCountries,
                Organisations = organisations.ToArray()
            };
        }
    }
}
