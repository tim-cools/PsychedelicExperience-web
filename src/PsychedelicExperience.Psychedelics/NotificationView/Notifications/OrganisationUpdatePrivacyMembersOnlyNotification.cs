using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class OrganisationUpdatePrivacyMembersOnlyNotification : Notification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Guid UpdateId { get; set; }
        public Guid OrganisationId { get; set; }

        public OrganisationUpdatePrivacyMembersOnlyNotification()
        {
        }

        public OrganisationUpdatePrivacyMembersOnlyNotification(Guid userId, Context<OrganisationUpdatePrivacyChanged> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            UpdateId = (Guid)context.Event.UpdateId;
            OrganisationId = (Guid)context.Event.OrganisationId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" changed the privacy level of your organisation update ")
            .Link(Name, $"/organisation/{OrganisationId}/update/{UpdateId}")
            .Text(" to members only.");
    }
}