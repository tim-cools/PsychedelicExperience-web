using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.TopicInteractionView;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    public class GetEventsHandler : QueryHandler<GetEvents, EventsResult>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetEventsHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<EventsResult> Execute(GetEvents getEventsQuery)
        {
            var user = await Session.LoadUserAsync(getEventsQuery.UserId);

            var organisationIds = await OrganisationIds(user);

            var criteria = PredicateBuilder.New<Event>()
                .WithPrivacy(user, organisationIds)
                .FilterByUser(getEventsQuery.FilterByUser, organisationIds)
                .FilterType(getEventsQuery.EventType)
                .FilterOrganisation(getEventsQuery.OrganisationId)
                .FilterQueryString(getEventsQuery.Query)
                .FilterTags(getEventsQuery.Tags)
                .FilterCountry(getEventsQuery.Country)
                .FilterDate();

            var query = new GetEventsQueryBuilder(user, criteria, Session, _userInfoResolver)
                .Paging(getEventsQuery.Page);

            var result = await query.Execute();
            FillOrganisationDetails(getEventsQuery, result);
            return result;
        }

        private void FillOrganisationDetails(GetEvents getEventsQuery, EventsResult result)
        {
            if (getEventsQuery.OrganisationId == null) return;

            var organisation = Session.Load<Organisation>(getEventsQuery.OrganisationId);
            if (organisation == null)
            {
                throw new InvalidOperationException($"Organisation not found: {getEventsQuery.OrganisationId}");
            }
            
            result.OrganisationName = organisation.Name;
            result.OrganisationUrl = organisation.GetUrl();
        }

        private async Task<List<Guid>> OrganisationIds(User user)
        {
            if (user == null) return new List<Guid>();

            var ownedOrganisations = await Session.Query<Organisation>()
                .Where(organisation => organisation.Owners.Contains(user.Id))
                .Select(organisation => organisation.Id)
                .ToListAsync();

            var followedOrganisations = await Session
                .Query<TopicFollower>()
                .Where(follower => follower.UserId == user.Id)
                .Select(organisation => organisation.Id)
                .ToListAsync();

            return ownedOrganisations.Union(followedOrganisations).Distinct().ToList();
        }
    }

    //public class GetEventMembersHandler : QueryHandler<GetEventMembers, EventMembersResult>
    //{
    //    private readonly IUserInfoResolver _userInfoResolver;

    //    public GetEventMembersHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
    //    {
    //        _userInfoResolver = userInfoResolver;
    //    }

    //    protected override async Task<EventMembersResult> Execute(GetEventMembers getEventsQuery)
    //    {
    //        var user = await Session.LoadUserAsync(getEventsQuery.UserId);
    //        var @event = await Session.LoadAsync<Event>(getEventsQuery.EventId);

    //        var eventId = (Guid) getEventsQuery.EventId;

    //        var members = await Session
    //            .Query<EventMember>()
    //            .Where(follower => follower.EventId == eventId)
    //            .ToListAsync();

    //        var memebers = await members.MapMembers(user, @event);

    //        return new EventMembersResult
    //        {
                
    //        }
    //    }
    //  }
}
