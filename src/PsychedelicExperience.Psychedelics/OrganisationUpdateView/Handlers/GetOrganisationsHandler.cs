using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.TopicInteractionView;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdateView.Handlers
{
    public class GetOrganisationUpdatesHandler : QueryHandler<GetOrganisationUpdates, OrganisationUpdatesResult>
    {
        private readonly IUserInfoResolver _userInforesolver;

        public GetOrganisationUpdatesHandler(IQuerySession session, IUserInfoResolver userInforesolver) : base(session)
        {
            _userInforesolver = userInforesolver;
        }

        protected override async Task<OrganisationUpdatesResult> Execute(GetOrganisationUpdates getOrganisationUpdates)
        {
            var user = await Session.LoadUserAsync(getOrganisationUpdates.UserId);

            var organisationId = (Guid) getOrganisationUpdates.OrganisationId;
            var subscription = await GetSubscription(getOrganisationUpdates.UserId, organisationId);

            var organisation = await Session.LoadAsync<Organisation>(organisationId);

            if (organisation == null)
            {
                throw new InvalidOperationException($"Organisation not found: {organisationId}");
            }

            var query = new GetOrganisationUpdatesQueryBuilder(Session, _userInforesolver)
                .IncludeOrganisation(getOrganisationUpdates.IncludeOrganisation)
                .WithPrivacy(user, subscription, organisation.IsOwner(user))
                .ForOrganisation(getOrganisationUpdates.OrganisationId)
                .Paging(getOrganisationUpdates.Page);

            return await query.Execute();
        }

        private async Task<bool> GetSubscription(UserId userId, Guid organisationId)
        {
            if (userId == null) return false;

            var userIdValue = (Guid)userId;

            return await Session
                       .Query<TopicFollower>()
                       .CountAsync(follower => follower.UserId == userIdValue && follower.TopicId == organisationId) > 0;
        }
    }
}
