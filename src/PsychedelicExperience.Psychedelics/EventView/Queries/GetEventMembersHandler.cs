using System;
using System.Collections.Generic;
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
    public class GetEventMembersHandler : QueryHandler<GetEventMembers, EventMembersResult>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetEventMembersHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<EventMembersResult> Execute(GetEventMembers query)
        {
            var @event = await SessionExtensions.LoadAsync<Event>(Session, query.EventId);
            if (@event == null || @event.Removed)
            {
                return null;
            }

            var isOrganisationMember = await IsOrganisationMember(query.UserId, (Guid)query.EventId);

            var organisation = await Session.LoadAsync<Organisation>(@event.OrganisationId);
            if (organisation == null || organisation.Removed)
            {
                return null;
            }

            var user = await SessionExtensions.LoadUserAsync(Session, query.UserId);

            if (!@event.CanView(user, isOrganisationMember, organisation.IsOwner(user)))
            {
                return null;
            }

            var members = (await GetEventMembers(query)).MapMembers(_userInfoResolver);
            return new EventMembersResult { Members = members };
        }

        private async Task<IReadOnlyList<EventMember>> GetEventMembers(GetEventMembers query)
        {
            if (query.UserId == null) return null;

            var eventId = query.EventId.Value;

            return await Queryable.Where<EventMember>(Session.Query<EventMember>(), where => where.EventId == eventId && where.Status != EventMemberStatus.Removed)
                .ToListAsync();
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