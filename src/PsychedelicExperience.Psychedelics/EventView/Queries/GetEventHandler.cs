using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.TopicInteractionView;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    public class GetEventHandler : QueryHandler<GetEvent, EventDetails>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetEventHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<EventDetails> Execute(GetEvent query)
        {
            var @event = await Session.LoadAsync<Event>(query.EventId);
            if (@event == null || @event.Removed)
            {
                return null;
            }

            var organisation = await Session.LoadAsync<Organisation>(@event.OrganisationId);

            var user = await Session.LoadUserAsync(query.UserId);
            var isOrganisationMember = await IsOrganisationMember(query.UserId, (Guid)query.EventId);

            var member = await GetEventMember(query);

            return @event.MapDetails(organisation, user, isOrganisationMember, member, _userInfoResolver);
        }

        private async Task<EventMember> GetEventMember(GetEvent query)
        {
            if (query.UserId == null) return null;

            var userId = query.UserId.Value;
            var eventId = query.EventId.Value;

            return await Session.Query<EventMember>()
                .Where(where => where.MemberId == userId && where.EventId == eventId)
                .SingleOrDefaultAsync();
        }

        private async Task<bool> IsOrganisationMember(UserId userId, Guid organisationId)
        {
            if (userId == null) return false;

            var userIdValue = (Guid)userId;

            return await Session
                .Query<TopicFollower>()
                .CountAsync(follower => follower.UserId == userIdValue && follower.TopicId == organisationId) > 0;
        }
    }
}
