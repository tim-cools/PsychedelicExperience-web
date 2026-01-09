using System.Globalization;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Addresses;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationsHandler : QueryHandler<GetOrganisations, OrganisationsResult>
    {
        private readonly IUserInfoResolver _userInfoResolver;
        private readonly ICountryMapper _countryMapper;

        public GetOrganisationsHandler(IQuerySession session, IUserInfoResolver userInfoResolver, ICountryMapper countryMapper) : base(session)
        {
            _userInfoResolver = userInfoResolver;
            _countryMapper = countryMapper;
        }

        protected override async Task<OrganisationsResult> Execute(GetOrganisations getOrganisationsQuery)
        {
            var user = await Session.LoadUserAsync(getOrganisationsQuery.UserId);

            var query = new GetOrganisationsQueryBuilder(Session, _userInfoResolver)
                .WithPrivacy(user)
                .Format(getOrganisationsQuery.Format)
                .FilterTypes(getOrganisationsQuery.Types)
                .FilterByHasOwner(getOrganisationsQuery.HasOwner)
                .FilterByUser(getOrganisationsQuery.FilterByUser, user)
                .FilterCountry(_countryMapper.GetCode(getOrganisationsQuery.Country))
                .FilterQueryString(getOrganisationsQuery.Query)
                .FilterTags(getOrganisationsQuery.Tags, getOrganisationsQuery.OnlyWithoutTags)
                .Paging(getOrganisationsQuery.Page);

            return await query.Execute();
        }
    }
}
