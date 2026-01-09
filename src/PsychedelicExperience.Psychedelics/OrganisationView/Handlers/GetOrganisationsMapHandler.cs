
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Addresses;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationsMapHandler : QueryHandler<GetOrganisationsMap, OrganisationMapPoint[]>
    {
        private readonly ICountryMapper _countryMapper;

        public GetOrganisationsMapHandler(IQuerySession session, ICountryMapper countryMapper) : base(session)
        {
            _countryMapper = countryMapper;
        }

        protected override async Task<OrganisationMapPoint[]> Execute(GetOrganisationsMap getOrganisationsQuery)
        {
            var user = await Session.LoadUserAsync(getOrganisationsQuery.UserId);

            var query = new GetOrganisationsMapQueryBuilder(Session)
                .WithPrivacy(user)
                .FilterCountry(_countryMapper.GetCode(getOrganisationsQuery.Country))
                .FilterTags(getOrganisationsQuery.Tags);

            return await query.Execute();
        }
    }
}
