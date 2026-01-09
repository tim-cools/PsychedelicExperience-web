using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries
{
    public class OrganisationUpdateSummary
    {
        public ShortGuid OrganisationUpdateId { get; set; }

        public string Url { get; set; }

        public ShortGuid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationUrl { get; set; }

        public string PrivacyLevel { get; set; }

        public DateTime Created { get; set; }
        public string CreatedUserName { get; set; }
        public ShortGuid CreatedUserId { get; set; }

        public DateTime LastUpdated { get; set; }
        public string LastUpdatedUserName { get; set; }
        public ShortGuid LastUpdatedUserId { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }
    }
}