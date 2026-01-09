using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.EventView.Queries;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.TopicInteractionView;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    public class GetEventSummaryHandler : QueryHandler<GetEventSummary, EventSummary>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetEventSummaryHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<EventSummary> Execute(GetEventSummary query)
        {
            var @event = await Session.LoadAsync<Event>(query.EventId);
            if (@event == null || @event.Removed)
            {
                return null;
            }

            var organisation = await Session.LoadAsync<Organisation>(@event.OrganisationId);

            var user = await SessionExtensions.LoadUserAsync(Session, query.UserId);
            var isOrganisationMember = await IsOrganisationMember(query.UserId, (Guid)query.EventId);

            var member = await GetEventMember(query);

            return @event.MapSummary(organisation, user, isOrganisationMember, member, _userInfoResolver);
        }

        private async Task<EventMember> GetEventMember(GetEventSummary query)
        {
            if (query.UserId == null) return null;

            var userId = query.UserId.Value;
            var eventId = query.EventId.Value;

            return await Queryable.Where<EventMember>(Session.Query<EventMember>(), where => where.MemberId == userId && where.EventId == eventId)
                .SingleOrDefaultAsync();
        }

        private async Task<bool> IsOrganisationMember(UserId userId, Guid organisationId)
        {
            if (userId == null) return false;

            var userIdValue = (Guid)userId;

            return await QueryableExtensions.CountAsync<TopicFollower>(Session
                           .Query<TopicFollower>(), follower => follower.UserId == userIdValue && follower.TopicId == organisationId) > 0;
        }
    }
}