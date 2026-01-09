using System;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationSummaryHandler : QueryHandler<GetOrganisationSummary, OrganisationSummary>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetOrganisationSummaryHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<OrganisationSummary> Execute(GetOrganisationSummary query)
        {
            var id = (Guid)query.OrganisationId;

            var organisation = await Session.LoadAsync<Organisation>(id);

            var user = await Session.LoadUserAsync(query.UserId);

            return organisation?.MapSummary(user, _userInfoResolver);
        }
    }
}