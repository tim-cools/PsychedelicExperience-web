using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using Marten.Linq;
using Marten.Services.Includes;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    public class GetEventsQueryBuilder
    {
        public const int PageSize = 20;

        private readonly ExpressionStarter<Event> _where;
        private readonly User _user;
        private readonly IQuerySession _session;
        private readonly IUserInfoResolver _userInfoResolver;
        private int _page;

        public GetEventsQueryBuilder(User user, ExpressionStarter<Event> criteria, IQuerySession session, IUserInfoResolver userInfoResolver)
        {
            _user = user;
            _session = session;
            _userInfoResolver = userInfoResolver;
            _where = criteria;
        }

        public GetEventsQueryBuilder Paging(int page)
        {
            _page = page;
            return this;
        }

        private EventSummary[] Map(IReadOnlyList<Event> events, IReadOnlyList<Organisation> organisations, IReadOnlyList<EventMember> eventMembers, IReadOnlyList<TopicFollower> followingOrganisation)
        {
            return events
                .Select(@event => new
                {
                    Event = @event,
                    Organisation = organisations.FirstOrDefault(organisation => organisation.Id == @event.OrganisationId),
                    Member = eventMembers.FirstOrDefault(member => member?.EventId == @event.Id),
                    IsOrganisationMember = followingOrganisation.Any(organisation => organisation.TopicId == @event.OrganisationId)
                })
                .Select(value => value.Event.MapSummary(value.Organisation, _user, value.IsOrganisationMember, value.Member, _userInfoResolver))
                .ToArray();
        }

        public async Task<EventsResult> Execute()
        {
            var interactions = new List<TopicInteraction>();

            var skip = _page * PageSize;

            var events = await LoadEvents(interactions, skip, out var stats);
            var members = await LoadMembers(events);
            var organisations = await LoadOrganisations(events);
            var followingOrganisation = await LoadFollowingOrganisations(events);

            return new EventsResult
            {
                Events = Map(events, organisations, members, followingOrganisation),
                Page = _page,
                Total = stats.TotalResults,
                Last = skip + events.Count
            };
        }

        private Task<IReadOnlyList<Event>> LoadEvents(List<TopicInteraction> interactions, int skip, out QueryStatistics stats)
        {
            return _session.Query<Event>()
                .Stats(out stats)
                .Include(@event => @event.Id, interactions, JoinType.LeftOuter)
                .Where(_where)
                .OrderBy(@event => @event.StartDateTime)
                .Skip(skip)
                .Take(PageSize)
                .ToListAsync();
        }

        private async Task<IReadOnlyList<Organisation>> LoadOrganisations(IReadOnlyList<Event> events)
        {
            var organisationsIds = events.Select(@event => @event.OrganisationId).Distinct().ToList();

            return await _session.Query<Organisation>()
                .Where(where => organisationsIds.Contains(where.Id) && !where.Removed)
                .ToListAsync();
        }

        private async Task<IReadOnlyList<EventMember>> LoadMembers(IReadOnlyList<Event> events)
        {
            if (_user == null) return new List<EventMember>();

            var eventIds = events.Select(@event => @event.Id).ToList();
            var userId = _user.Id;

            return await _session.Query<EventMember>()
                .Where(where => where.MemberId == userId && eventIds.Contains(where.EventId))
                .ToListAsync();
        }


        private async Task<IReadOnlyList<TopicFollower>> LoadFollowingOrganisations(IReadOnlyList<Event> events)
        {
            if (_user == null) return new List<TopicFollower>();

            var organisationIds = events.Select(@event => @event.OrganisationId).ToList();
            var userId = _user.Id;

            return await _session
                .Query<TopicFollower>()
                .Where(follower => follower.UserId == userId && organisationIds.Contains(follower.TopicId))
                .ToListAsync();
        }
    }

    public static class EventQueryExtensions
    {
        public static ExpressionStarter<Event> FilterCountry(this ExpressionStarter<Event> expression, string country)
        {
            if (string.IsNullOrWhiteSpace(country)) return expression;

            country = country.NormalizeForSearch();

            return expression.And(experience => experience.Address.Country == country);
        }

        public static ExpressionStarter<Event> WithPrivacy(this ExpressionStarter<Event> expression, User user, IList<Guid> organisationIds)
        {
            if (user != null && user.IsAtLeast(Roles.ContentManager))
            {
                return expression.And(@event => !@event.Removed);
            }
            if (organisationIds.Count > 0)
            {
                return expression.And(@event => !@event.Removed
                                                && (@event.Privacy == EventPrivacy.Public
                                                    || @event.Privacy == EventPrivacy.MembersOnly && organisationIds.Contains(@event.OrganisationId)));
            }
            return expression.And(@event => !@event.Removed && @event.Privacy == EventPrivacy.Public);
        }

        public static ExpressionStarter<Event> FilterByUser(this ExpressionStarter<Event> expression, bool filter, IList<Guid> organisationIds)
        {
            if (filter && organisationIds.Count > 0)
            {
                return expression.And(@event => organisationIds.Contains(@event.OrganisationId));
            }
            return expression;
        }

        public static ExpressionStarter<Event> FilterType(this ExpressionStarter<Event> expression, Messages.Events.EventType? eventType)
        {
            if (eventType == null) return expression;
            var value = eventType.Value.CastByName<EventType>();
            return expression.And(@event => @event.EventType == value);
        }

        public static ExpressionStarter<Event> FilterQueryString(this ExpressionStarter<Event> expression, string queryString)
        {
            queryString = queryString.NormalizeForSearch();

            if (string.IsNullOrWhiteSpace(queryString)) return expression;

            var query = PredicateBuilder.New<Event>()
                .Or(@event => @event.SearchString.Contains(queryString))
                .Or(@event => @event.TagsNormalized.Contains(queryString));

            return expression.And(query);
        }

        public static ExpressionStarter<Event> FilterOrganisation(this ExpressionStarter<Event> expression, OrganisationId organisationId)
        {
            if (organisationId == null) return expression;

            var value = organisationId.Value;
            return expression.And(@event => @event.OrganisationId == value);
        }

        public static ExpressionStarter<Event> FilterTags(this ExpressionStarter<Event> expression, string[] tags)
        {
            if (tags == null || tags.Length == 0) return expression;

            foreach (var tag in tags)
            {
                var value = tag.NormalizeForSearch();
                return expression.And(@event => @event.TagsNormalized.Contains(value));
            }
            return expression;
        }

        public static ExpressionStarter<Event> FilterDate(this ExpressionStarter<Event> expression)
        {
            var now = DateTime.Now.Date;
            return expression.And(@event => @event.StartDateTime == null 
                || @event.StartDateTime > now
                || @event.EndDateTime.HasValue && @event.EndDateTime > now);
        }
    }
}