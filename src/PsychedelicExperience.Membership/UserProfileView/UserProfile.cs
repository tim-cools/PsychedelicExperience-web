using System;
using System.Collections.Generic;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership.UserProfileView
{
    public class UserProfile : AggregateRoot
    {
        public IList<Guid> Owners { get; } = new List<Guid>();
        public IList<Role> Roles { get; } = new List<Role>();

        public UserId CreatorId { get; set; }
        public bool EmailConfirmed { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string EMail { get; set; }
        public string TagLine { get; set; }
        public string Description { get; set; }
        public Avatar Avatar { get; set; }
        public NotificationEmail NotificationEmail { get; set; } = new NotificationEmail();

        public bool CanView(User user) => true;

        public bool Is(User user) => user != null && Id == user.Id;
    }

    public class NotificationEmail
    {
        public bool Enabled { get; set; } = true;
        public TimeSpan Interval { get; set; } = TimeSpan.FromDays(1);
    }

    public class Avatar
    {
        public Guid AvatarId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }

        public Avatar(Guid avatarId, string fileName, string originalFileName)
        {
            AvatarId = avatarId;
            FileName = fileName;
            OriginalFileName = originalFileName;
        }
    }
}