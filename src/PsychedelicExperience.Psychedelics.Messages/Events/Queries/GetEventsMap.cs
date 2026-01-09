using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class GetEventsMap : IRequest<EventMapPoint[]>
    {
        public UserId UserId { get; }
        public EventType? EventType { get; }
        public string Country { get; }
        public string Query { get; }
        public int Page { get; }
        public bool FilterByUser { get; }
        public OrganisationId OrganisationId { get; }
        public string[] Tags { get; set; }

        public GetEventsMap(UserId userId, EventType? eventType = null, string country = null, string[] tags = null, string query = null, int page = 0, bool filterByUser = false, OrganisationId organisationId = null)
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
}