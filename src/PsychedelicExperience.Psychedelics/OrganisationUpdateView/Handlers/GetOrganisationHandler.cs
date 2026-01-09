using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.OrganisationView.Handlers;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdateView.Handlers
{
    public class GetOrganisationUpdateHandler : QueryHandler<GetOrganisationUpdate, OrganisationUpdateResult>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetOrganisationUpdateHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<OrganisationUpdateResult> Execute(GetOrganisationUpdate query)
        {
            var id = (Guid) query.OrganisationId;
            var updateId = (Guid) query.UpdateId;

            var organisation = await Session.LoadAsync<Organisation>(id);
            var update = await Session.LoadAsync<OrganisationUpdate>(updateId);
            var user = await Session.LoadUserAsync(query.UserId);
            var subscription = await GetSubscription(query.UserId, id);

            return new OrganisationUpdateResult
            {
                Organisation = query.IncludeOrganisation ? await GetOrganisation(organisation, user) : null, 
                Update = update?.MapDetails(user, organisation, _userInfoResolver, subscription, user?.IsAdministrator() == true)
            };
        }

        private async Task<OrganisationDetails> GetOrganisation(Organisation organisation, User user)
        {
            var reviews = await Session.Query<Review>()
                .Where(review => review.OrganisationId == organisation.Id && !review.Removed)
                .ToListAsync();

            var relatedOrganisationIds = organisation.RelationOrganisationIds();

            var relatedOrganisations = await Session.LoadManyAsync<Organisation>(relatedOrganisationIds);

            var relatedOrganisationDictionary = relatedOrganisations.ToDictionary(value => value.Id, value => value);

            return organisation?.MapDetails(
                user,
                reviews,
                relatedOrganisationDictionary,
                _userInfoResolver);
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
