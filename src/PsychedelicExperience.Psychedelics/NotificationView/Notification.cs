using System;
using System.Collections.Generic;
using PsychedelicExperience.Membership.UserInfo;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    public abstract class Notification
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateTime { get; set; }

        protected Notification()
        {
        }

        protected Notification(Guid userId, DateTime dateTime)
        {
            UserId = userId;
            DateTime = dateTime;
        }

        public abstract Markup Format(IUserInfoResolver userInfoResolver);
    }

    public abstract class GroupNotification : Notification
    {
        protected GroupNotification()
        {
        }

        protected GroupNotification(Guid userId, DateTime dateTime) : base(userId, dateTime)
        {
        }

        public abstract Markup GroupFormat(IEnumerable<GroupNotification> notifications);
    }
}