using System;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class UserToRoleAddedNotification : Notification
    {
        public Guid? RequesterId { get; set; }
        public Guid ChangedUserId { get; set; }
        public string Role { get; set; }

        public UserToRoleAddedNotification()
        {
        }

        public UserToRoleAddedNotification(Guid userId, Context<UserToRoleAdded> context)
            : base(userId, context.Event.EventTimestamp)
        {
            ChangedUserId = (Guid) context.Event.UserToChangeId;
            RequesterId = context.Event.RequesterId == null ? (Guid?) null : (Guid) context.Event.RequesterId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" granted you the role ")
            .Link(Role, $"/user/{ChangedUserId}")
            .Text(".");
    }
}