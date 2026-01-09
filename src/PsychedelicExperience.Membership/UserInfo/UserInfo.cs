using System;

namespace PsychedelicExperience.Membership.UserInfo
{
    public class UserInfo
    {
        public Guid? UserId { get; }
        public string DisplayName { get; }

        public UserInfo(Guid? userId, string displayName)
        {
            UserId = userId;
            DisplayName = displayName;
        }
    }
}