using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class ExperienceAddedNotification : Notification
    {
        public Guid ExperienceId { get; set; }
        public Guid ChangedUserId { get; set; }
        public string Title { get; set; }

        public ExperienceAddedNotification()
        {
        }

        public ExperienceAddedNotification(Guid userId, Context<ExperienceAdded> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Title = context.StatusView.Name;
            ChangedUserId = (Guid) context.Event.UserId;
            ExperienceId = (Guid) context.Event.ExperienceId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" added an experience ")
            .Link(Title, $"/experience/{ExperienceId}")
            .Text(".");
    }
}