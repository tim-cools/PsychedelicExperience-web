using System;
using Marten.Events.Projections;
using PsychedelicExperience.Membership.Messages.UserProfiles;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    public class UserStatusProjection : ViewProjection<NotificationUserStatus, Guid>
    {
        public UserStatusProjection()
        {
            Configure();
        }

        private void Configure()
        {
            ProjectEvent<UserProfileCreated>((view, @event) =>
            {
                view.UserId = (Guid) @event.UserProfileId;
                view.Name = @event.DisplayName?.Value;
                view.Email = @event.EMail?.Value;
                view.Enabled = true;
                view.Interval = TimeSpan.FromDays(7);
            });
            
            ProjectEvent<UserProfileDisplayNameChanged>((view, @event) => view.Name = @event.DisplayName?.Value);
            ProjectEvent<UserProfileEMailChanged>((view, @event) => view.Email = @event.EMail?.Value);

            ProjectEvent<UserProfileNotificationEmailDisabled>((view, @event) => view.Enabled = false);
            ProjectEvent<UserProfileNotificationEmailEnabled>((view, @event) =>
            {
                view.Enabled = true;
                view.Expire = DateTime.Now + view.Interval;
            });

            ProjectEvent<UserProfileNotificationEmailIntervalChanged>((view, @event) =>
            {
                view.Interval = @event.Interval;
                view.Expire = view.DateTime + view.Interval;
            });
        }
    }
}