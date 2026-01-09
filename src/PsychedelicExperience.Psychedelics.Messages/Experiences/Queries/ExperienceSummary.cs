using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class ExperienceSummary
    {
        public ShortGuid ExperienceId { get; set; }
        public string Url { get; set; }

        public ShortGuid UserId { get; set; }
        public string UserName { get; set; }

        public DateTime? Created { get; set; }

        public string Title { get; set; }
        public DateTime? DateTime { get; set; }
        public string PrivacyLevel { get; set; }

        public string[] Substances { get; set; }
        public string[] Tags { get; set; }

        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public int CommentCount { get; set; }
    }
}