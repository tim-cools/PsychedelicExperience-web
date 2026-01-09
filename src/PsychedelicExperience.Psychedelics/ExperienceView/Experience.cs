using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Security;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.ExperienceView
{
    public class Experience
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Title { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }


        public string SetDescription { get; set; }
        public string SettingDescription { get; set; }
        public string PublicDescription { get; set; }
        public EncryptedString PrivateDescription { get; set; }
        public string SearchString { get; set; }

        public string Level { get; set; }
        public PrivacyLevel PrivacyLevel { get; set; }

        public IList<string> Partners { get; } = new List<string>();
        public IList<Dose> Doses { get; } = new List<Dose>();

        public IList<string> Tags { get; } = new List<string>();
        public IList<string> TagsNormalized { get; } = new List<string>();

        public bool IsOwner(User user)
        {
            return user != null && user.Id == UserId;
        }

        public bool CanView(User user)
        {
            return PrivacyLevel != PrivacyLevel.Private
                || IsOwner(user);
        }

        public bool IsRestricted(User user)
        {
            return PrivacyLevel == PrivacyLevel.Restricted
                && !IsOwner(user);
        }

        public Dose GetDose(Guid doseId)
        {
            var dose = Doses.FirstOrDefault(where => where.Id == doseId);
            if (dose == null)
            {
                throw new InvalidOperationException("Dose not found:" + doseId);
            }
            return dose;
        }

        public string Slug() => Title.NormalizeForUrl();
    }
}