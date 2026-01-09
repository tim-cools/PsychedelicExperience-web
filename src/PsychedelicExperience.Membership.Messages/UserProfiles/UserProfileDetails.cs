using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.UserProfiles
{
    public class UserProfileDetails
    {
        public ShortGuid UserProfileId { get; set; }
        public bool EmailConfirmed { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string TagLine { get; set; }
        public string Description { get; set; }
        public string[] Roles { get; set; }

        public string EMail { get; set; }

        public NotificationEmail NotificationEmail { get; set; }

        public UserProfilePrivileges Privileges { get; set; }
    }

    public class NotificationEmail
    {
        public bool Enabled { get; set; }
        public int Minutes { get; set; }
    }

    public class UserProfilePrivileges
    {
        public bool IsAdministrator { get; set; }
        public bool Editable { get; set; }
        public bool IsOwner { get; set; }
    }
}