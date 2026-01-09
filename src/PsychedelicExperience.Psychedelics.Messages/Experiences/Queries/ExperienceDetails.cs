using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class ExperienceDetails
    {
        public ShortGuid ExperienceId { get; set; }
        public ShortGuid UserId { get; set; }

        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string UserAvatar { get; set; }

        public DateTime? Created { get; set; }

        public string Title { get; set; }
        public DateTime? DateTime { get; set; }
        public string ExternalDescription { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
        public string Partner { get; set; }

        public string Level { get; set; }

        public string Set { get; set; }
        public string Setting { get; set; }
        public string PrivateNotes { get; set; }
        public string PublicDescription { get; set; }
        public string PrivacyLevel { get; set; }

        public string[] Tags { get; set; }
        public Dose[] Doses { get; set; }

        public ExperienceDetailsPrivileges Privileges { get; set; }
    }
}