using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class EventsResult
    {
        public EventSummary[] Events { get; set; }
        public long Page { get; set; }
        public long Total { get; set; }
        public long Last { get; set; }

        public string OrganisationName { get; set; }
        public string OrganisationUrl { get; set; }
    }

    public class EventsMapResult
    {
        public EventMapPoint[] Organisations { get; set; }
    }

    public class EventMapPoint
    {
        public string Name { get; set; }
        public ShortGuid Id { get; set; }
        public Position Position { get; set; }
    }

    public class EventMembersResult
    {
        public EventMember[] Members { get; set; }
        public long Page { get; set; }
        public long Total { get; set; }
        public long Last { get; set; }
    }

    public class EventMember
    {
        public ShortGuid UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
    }

    public class HubspotDefaultsResult
    {
        public CompanyData Company { get; set; }
        public OrganisationData[] Existing { get; set; }
    }

    public class OrganisationData
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class CompanyData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string LinkedIn { get; set; }
        public string YouTube { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}