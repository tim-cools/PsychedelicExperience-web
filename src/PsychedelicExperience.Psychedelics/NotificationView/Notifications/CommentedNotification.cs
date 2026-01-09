using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class CommentedNotification : Notification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Topic Topic { get; set; }

        public CommentedNotification()
        {
        }

        public CommentedNotification(Guid userId, Context<Commented> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            Topic = context.StatusView.Topic;
        }
        
        public override Markup Format(IUserInfoResolver userInfoResolver)
        {
            return new Markup()
                .User(ChangedUserId, UserId, userInfoResolver)
                .Text($" commented your {Topic.TypeText()} ")
                .Link(Name, Topic.Url())
                .Text(".");
        }
    }
}