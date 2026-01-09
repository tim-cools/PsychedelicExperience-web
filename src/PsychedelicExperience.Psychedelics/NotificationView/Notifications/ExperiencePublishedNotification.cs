using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class ExperiencePublishedNotification : Notification
    {
        public Guid ExperienceId { get; set; }
        public Guid ChangedUserId { get; set; }
        public string Title { get; set; }

        public ExperiencePublishedNotification()
        {
        }

        public ExperiencePublishedNotification(Guid userId, Context<ExperiencePrivacyLevelChanged> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Title = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            ExperienceId = (Guid)context.Event.ExperienceId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" published your experience ")
            .Link(Title, $"/experience/{ExperienceId}")
            .Text(" to be visible in public.");
    }
}