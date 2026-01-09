using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Messages.Users;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership.UserProfiles
{
    public class UserProfile : AggregateRoot
    {
        public UserId CreatorId { get; set; }
        public EMail EMail { get; set; }

        public Name DisplayName { get; set; }
        public Name FullName { get; set; }

        public string TagLine { get; set; }
        public Description Description { get; set; }

        public Avatar Avatar { get; set; }
        public NotificationEmail NotificationEmail { get; set; } = new NotificationEmail();

        public void Handle(User user, CreateUserProfile command)
        {
            Publish(new UserProfileCreated
            {
                UserProfileId = command.UserProfileId,
                CreatorId = command.RequesterId,
                DisplayName = command.DisplayName,
                FullName = command.FullName,
                EMail = command.EMail
            });
        }

        public void Apply(UserProfileCreated @event)
        {
            Id = (Guid)@event.UserProfileId;
            CreatorId = @event.CreatorId;
            DisplayName = @event.DisplayName;
            FullName = @event.FullName;
            EMail = @event.EMail;
        }

        public void Handle(User user, ChangeUserProfileEMail command)
        {
            Publish(new UserProfileEMailChanged
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId,
                EMail = command.EMail,
                DisplayName = DisplayName
            });
        }

        public void Apply(UserProfileEMailChanged @event)
        {
            EMail = @event.EMail;
        }

        public void Handle(User user, ChangeUserProfileDisplayName command)
        {
            Publish(new UserProfileDisplayNameChanged
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId,
                DisplayName = command.Name
            });
        }

        public void Apply(UserProfileDisplayNameChanged @event)
        {
            DisplayName = @event.DisplayName;
        }

        public void Handle(User user, ChangeUserProfileFullName command)
        {
            Publish(new UserProfileFullNameChanged
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId,
                FullName = command.Name
            });
        }

        public void Apply(UserProfileFullNameChanged @event)
        {
            FullName = @event.FullName;
        }

        public void Handle(User user, ChangeUserProfileTagline command)
        {
            Publish(new UserProfileTaglineChanged
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId,
                TagLine = command.TagLine
            });
        }

        public void Apply(UserProfileTaglineChanged @event)
        {
            TagLine = @event.TagLine;
        }

        public void Handle(User user, ChangeUserProfileDescription command)
        {
            Publish(new UserProfileDescriptionChanged
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId,
                Description = command.Description
            });
        }

        public void Apply(UserProfileDescriptionChanged @event)
        {
            Description = @event.Description;
        }

        public void Handle(User user, ChangeUserProfileAvatar command)
        {
            Publish(new UserProfileAvatarChanged
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId,
                AvatarId = new AvatarId(command.File.Id),
                FileName = command.File.FileName,
                OriginalFileName = command.File.OriginalFileName
            });
        }

        public void Apply(UserProfileAvatarChanged @event)
        {
            Avatar = new Avatar(
                @event.AvatarId,
                @event.FileName,
                @event.OriginalFileName);
        }

        public void Handle(User user, DisableUserProfileNotificationEmail command)
        {
            Publish(new UserProfileNotificationEmailDisabled
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId
            });
        }

        public void Apply(UserProfileNotificationEmailDisabled @event)
        {
            NotificationEmail.Enabled = false;
        }

        public void Handle(User user, EnableUserProfileNotificationEmail command)
        {
            Publish(new UserProfileNotificationEmailEnabled
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId
            });
        }

        public void Apply(UserProfileNotificationEmailEnabled @event)
        {
            NotificationEmail.Enabled = true;
        }

        public void Handle(User user, ChangeUserProfileNotificationEmailInterval command)
        {
            Publish(new UserProfileNotificationEmailIntervalChanged
            {
                UserProfileId = command.UserProfileId,
                UserId = command.RequesterId,
                Interval = command.Interval
            });
        }

        public void Apply(UserProfileNotificationEmailIntervalChanged @event)
        {
            NotificationEmail.Interval = @event.Interval;
        }

        public bool IsOwner(User user)
        {
            return user != null && Id == user.Id;
        }

        public void EnsureCanEdit(User user)
        {
            if (!user.IsAdministrator() && !IsOwner(user))
            {
                throw new BusinessException($"{user.Id} can not edit user profile {Id}!");
            }
        }
    }

    public class NotificationEmail
    {
        public bool Enabled { get; set; }
        public TimeSpan Interval { get; set; }
    }

    public class Avatar
    {
        public AvatarId AvatarId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }

        public Avatar(AvatarId avatarId, string fileName, string originalFileName)
        {
            AvatarId = avatarId;
            FileName = fileName;
            OriginalFileName = originalFileName;
        }
    }
}