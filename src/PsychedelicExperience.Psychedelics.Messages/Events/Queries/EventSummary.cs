using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class EventSummary
    {
        public ShortGuid EventId { get; set; }
        public string Url { get; set; }

        public ShortGuid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationUrl { get; set; }

        public ShortGuid UserId { get; set; }
        public ShortGuid LastUpdatedUserId { get; set; }
        
        public string UserName { get; set; }
        public string LastUpdatedUserName { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? LastUpdated { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public string Type { get; set; }
        public string Privacy { get; set; }

        public string[] Tags { get; set; }
        public Members Members { get; set; }
    }
}