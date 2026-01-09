using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class EventDetails
    {
        public ShortGuid EventId { get; set; }

        public ShortGuid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationUrl { get; set; }

        public DateTime? Created { get; set; }
        public ShortGuid CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }

        public DateTime? LastUpdated { get; set; }
        public ShortGuid LastUpdatedUserId { get; set; }
        public string LastUpdatedUserName { get; set; }

        public string Name { get; set; }
        public string Privacy { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public string LocationName { get; set; }
        public EventAddress Address { get; set; }
        public string Url { get; set; }

        public EventDetailsPrivileges Privileges { get; set; }

        public string[] Tags { get; set; }
        public Members Members { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }

    public class Members
    {
        public string Status { get; set; }

        public int Invited { get; set; }
        public int Interested { get; set; }
        public int Attending { get; set; }
        public int NotAttending { get; set; }
    }
}