using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Queries
{
    public class DocumentDetails
    {
        public ShortGuid DocumentId { get; set; }

        public string Status { get; set; }
        public DateTime? Created { get; set; }
        public ShortGuid CreatedUserId { get; set; }
        public string CreatedUserName { get; set; }

        public DateTime? LastUpdated { get; set; }
        public ShortGuid LastUpdatedUserId { get; set; }
        public string LastUpdatedUserName { get; set; }

        public string Name { get; set; }
        public string Content { get; set; }
        public string ExternalDescription { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }

        public DocumentDetailsPrivileges Privileges { get; set; }

        public string[] Tags { get; set; }
    }
}