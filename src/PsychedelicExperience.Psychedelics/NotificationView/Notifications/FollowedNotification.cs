using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class FollowedNotification : GroupNotification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Topic Topic { get; set; }

        public FollowedNotification()
        {
        }

        public FollowedNotification(Guid userId, Context<Followed> context)
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
                .Text($" followed your {Topic.TypeText()} ")
                .Link(Name, Topic.Url())
                .Text(".");
        }

        public override Markup GroupFormat(IEnumerable<GroupNotification> notifications)
        {
            if (Topic == null) return null;

            var count = notifications.Count();

            return new Markup()
                .Text($"{count} user{(count > 1 ? "s" : string.Empty)} followed your {Topic.TypeText()} ")
                .Link(Name, Topic.Url())
                .Text(".");
        }
    }
}