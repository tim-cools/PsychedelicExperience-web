using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class OrganisationOwnerAddedNotification : Notification
    {
        public Guid OrganisationId { get; set; }
        public Guid ChangedUserId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }

        public OrganisationOwnerAddedNotification()
        {
        }

        public OrganisationOwnerAddedNotification(Guid userId, Context<OrganisationOwnerAdded> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            OwnerId = (Guid)context.Event.OwnerId;
            OrganisationId = (Guid)context.Event.OrganisationId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" added ")
            .User(OwnerId, UserId, userInfoResolver, ChangedUserId == UserId ? "yourself" : "you")
            .Text(" as owner of organisation ")
            .Link(Name, $"/organisation/{OrganisationId}")
            .Text(".");
    }
}