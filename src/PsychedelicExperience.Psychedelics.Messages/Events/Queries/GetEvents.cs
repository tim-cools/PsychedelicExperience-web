using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class GetEvents : IRequest<EventsResult>
    {
        public UserId UserId { get; }
        public EventType? EventType { get; }
        public string Country { get; }
        public string Query { get; }
        public int Page { get; }
        public bool FilterByUser { get; }
        public OrganisationId OrganisationId { get; }
        public string[] Tags { get; set; }

        public GetEvents(UserId userId, EventType? eventType = null, string country = null, string[] tags = null, string query = null, int page = 0, bool filterByUser = false, OrganisationId organisationId = null)
        {
            UserId = userId;
            EventType = eventType;
            Country = country;
            Tags = tags;
            Query = query;
            Page = page;
            FilterByUser = filterByUser;
            OrganisationId = organisationId;
        }
    }

    public class GetEventMembers : IRequest<EventMembersResult>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }

        public GetEventMembers(UserId userId, EventId eventId)
        {
            UserId = userId;
            EventId = eventId;
        }
    }

    public class GetHubspotDefaults : IRequest<HubspotDefaultsResult>
    {
        public UserId UserId { get; }
        public long HubspotId { get; }

        public GetHubspotDefaults(UserId userId, long hubspotId)
        {
            UserId = userId;
            HubspotId = hubspotId;
        }
    }
}