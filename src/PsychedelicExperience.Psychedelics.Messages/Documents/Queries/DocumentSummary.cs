using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Queries
{
    public class DocumentSummary
    {
        public ShortGuid DocumentId { get; set; }
        public string Url { get; set; }

        public ShortGuid UserId { get; set; }
        public ShortGuid LastUpdatedUserId { get; set; }
        
        public string UserName { get; set; }
        public string LastUpdatedUserName { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? LastUpdated { get; set; }

        public string Name { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }

        public string[] Tags { get; set; }

        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public int CommentCount { get; set; }

        public bool CanEdit { get; set; }
    }
}