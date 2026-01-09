using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Addresses;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.TopicInteractionView;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    public class GetEventsMapHandler : QueryHandler<GetEventsMap, EventMapPoint[]>
    {
        public GetEventsMapHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<EventMapPoint[]> Execute(GetEventsMap getOrganisationsQuery)
        {
            var user = await Session.LoadUserAsync(getOrganisationsQuery.UserId);

            var organisationIds = await OrganisationIds(user);

            var criteria = PredicateBuilder.New<Event>()
                .WithPrivacy(user, organisationIds)
                .FilterByUser(getOrganisationsQuery.FilterByUser, organisationIds)
                .FilterType(getOrganisationsQuery.EventType)
                .FilterOrganisation(getOrganisationsQuery.OrganisationId)
                .FilterQueryString(getOrganisationsQuery.Query)
                .FilterTags(getOrganisationsQuery.Tags)
                .FilterCountry(getOrganisationsQuery.Country)
                .FilterDate();

            var query = new GetEventsMapQueryBuilder(criteria, Session);

            return await query.Execute();
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
}