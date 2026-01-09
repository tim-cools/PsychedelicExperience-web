using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Addresses;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationCountriesHandler : QueryHandler<GetOrganisationCountries, Country[]>
    {
        private readonly ICountryMapper _countryMapper;

        public GetOrganisationCountriesHandler(
            IQuerySession session,
            ICountryMapper countryMapper) : base(session)
        {
            _countryMapper = countryMapper;
        }

        protected override async Task<Country[]> Execute(GetOrganisationCountries query)
        {
            var countries = await Session.Query<Organisation>()
                .Where(organisation => !organisation.Removed && organisation.Country != null)
                .Select(organisation => organisation.Country)
                .Distinct()
                .ToListAsync();

            var queryNormalized = query.QueryString?.ToLowerInvariant();

            bool Filter(Country country) =>
                queryNormalized == null
                || country.Name.ToLowerInvariant().IndexOf(queryNormalized, StringComparison.Ordinal) >= 0;

            Country Map(string code) =>
                new Country
                {
                    Code = code,
                    Name = _countryMapper.GetCountry(code)
                };

            return countries
                .Select(Map)
                .Where(Filter)
                .ToArray();
        }
    }
}