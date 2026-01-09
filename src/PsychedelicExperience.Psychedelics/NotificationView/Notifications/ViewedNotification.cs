using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class ViewedNotification : GroupNotification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Topic Topic { get; set; }

        public ViewedNotification()
        {
        }

        public ViewedNotification(Guid userId, Context<Viewed> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid) context.Event.UserId;
            Topic = context.StatusView.Topic;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text($" viewed your {Topic.TypeText()} ")
            .Link(Name, Topic.Url())
            .Text(".");

        public override Markup GroupFormat(IEnumerable<GroupNotification> notifications)
        {
            if (Topic == null) return null;

            var count = notifications.Count();

            var users = count > 1 ? $"{count} users" : "A user";

            return new Markup()
                .Text($"{users} viewed your {Topic.TypeText()} ")
                .Link(Name, Topic.Url())
                .Text(".");
        }
    }
}