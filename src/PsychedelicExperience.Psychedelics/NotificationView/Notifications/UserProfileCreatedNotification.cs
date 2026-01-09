using System;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.UserInfo;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class UserProfileCreatedNotification : Notification
    {
        public Guid UserProfileId { get; set; }

        public UserProfileCreatedNotification()
        {
        }

        public UserProfileCreatedNotification(Guid userId, Context<UserProfileCreated> context)
            : base(userId, context.Event.EventTimestamp)
        {
            UserProfileId = (Guid) context.Event.UserProfileId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .Text("Welcome to Psychedelic Experience!  ")
            .User(UserProfileId, UserId, userInfoResolver, "Your ")
            .Link("profile", $"/user/{UserProfileId}")
            .Text(" is created.");
    }
}