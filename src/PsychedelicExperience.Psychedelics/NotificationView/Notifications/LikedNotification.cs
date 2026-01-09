using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class LikedNotification : GroupNotification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Topic Topic { get; set; }

        public LikedNotification()
        {
        }

        public LikedNotification(Guid userId, Context<Liked> context)
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
                .Text($" liked your {Topic.TypeText()} ")
                .Link(Name, Topic.Url())
                .Text(".");
        }

        public override Markup GroupFormat(IEnumerable<GroupNotification> notifications)
        {
            if (Topic == null) return null;

            var count = notifications.Count();

            return new Markup()
                .Text($"{count} user{(count > 1 ? "s" : string.Empty)} liked your {Topic.TypeText()} ")
                .Link(Name, Topic.Url())
                .Text(".");
        }
    }
}