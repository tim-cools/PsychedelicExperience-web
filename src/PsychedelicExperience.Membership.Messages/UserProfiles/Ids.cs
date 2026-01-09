using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.UserProfiles
{
    public class UserProfileId : Id
    {
        public UserProfileId(Guid value) : base(value)
        {
        }

        public static UserProfileId New()
        {
            return new UserProfileId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator UserProfileId(Guid id)
        {
            return new UserProfileId(id);
        }
    }

    public enum PrivacyLevel
    {
        Private,
        Restricted,
        Public
    }

    public enum ExperiencePrivacyLevel
    {
        Private,
        Restricted,
        Public
    }
}