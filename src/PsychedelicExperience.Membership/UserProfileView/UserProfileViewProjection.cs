using System;
using Marten.Events.Projections;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Membership.UserProfileView
{
    public class UserProfileViewProjection : ViewProjection<UserProfile, Guid>
    {
        public UserProfileViewProjection()
        {
            ProjectEvent<UserToRoleAdded>(Project);
            ProjectEvent<UserFromRoleRemoved>(Project);
            ProjectEvent<UserEmailConfirmed>(Project);

            ProjectEvent<UserProfileCreated>(Project);
            ProjectEvent<UserProfileEMailChanged>(Project);
            ProjectEvent<UserProfileDisplayNameChanged>(Project);
            ProjectEvent<UserProfileFullNameChanged>(Project);
            ProjectEvent<UserProfileDescriptionChanged>(Project);
            ProjectEvent<UserProfileTaglineChanged>(Project);
            ProjectEvent<UserProfileAvatarChanged>(Project);

            ProjectEvent<UserProfileNotificationEmailDisabled>(Project);
            ProjectEvent<UserProfileNotificationEmailEnabled>(Project);
            ProjectEvent<UserProfileNotificationEmailIntervalChanged>(Project);
        }

        private void Project(UserProfile view, UserProfileCreated @event)
        {
            view.CreatorId = @event.CreatorId;
            view.DisplayName = (string)@event.DisplayName;
            view.FullName = (string)@event.FullName;
            view.EMail = @event.EMail?.Value.NormalizeForSearch();
        }

        private void Project(UserProfile view, UserToRoleAdded @event)
        {
            view.Roles.Add(@event.Role);
        }

        private void Project(UserProfile view, UserFromRoleRemoved @event)
        {
            view.Roles.Remove(@event.Role);
        }

        private void Project(UserProfile view, UserEmailConfirmed @event)
        {
            view.EmailConfirmed = true;
        }

        private void Project(UserProfile view, UserProfileEMailChanged @event)
        {
            view.EMail = (string) @event.EMail;
        }

        private void Project(UserProfile view, UserProfileDisplayNameChanged @event)
        {
            view.DisplayName = (string)@event.DisplayName;
        }

        private void Project(UserProfile view, UserProfileFullNameChanged @event)
        {
            view.FullName = (string)@event.FullName;
        }

        private void Project(UserProfile view, UserProfileDescriptionChanged @event)
        {
            view.Description = (string)@event.Description;
        }

        private void Project(UserProfile view, UserProfileTaglineChanged @event)
        {
            view.TagLine = @event.TagLine;
        }

        private void Project(UserProfile view, UserProfileAvatarChanged @event)
        {
            view.Avatar = new Avatar(
                @event.AvatarId.Value,
                @event.FileName,
                @event.OriginalFileName
                );
        }

        private void Project(UserProfile view, UserProfileNotificationEmailDisabled @event) 
            => view.NotificationEmail.Enabled = false;

        private void Project(UserProfile view, UserProfileNotificationEmailEnabled @event) 
            => view.NotificationEmail.Enabled = true;

        private void Project(UserProfile view, UserProfileNotificationEmailIntervalChanged @event) 
            => view.NotificationEmail.Interval = @event.Interval;
    }
}