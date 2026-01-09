using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class OrganisationOwnerRemovedNotification : Notification
    {
        public Guid OrganisationId { get; set; }
        public Guid ChangedUserId { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }

        public OrganisationOwnerRemovedNotification()
        {
        }

        public OrganisationOwnerRemovedNotification(Guid userId, Context<OrganisationOwnerRemoved> context)
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
            .User(OwnerId, UserId, userInfoResolver, "yourself")
            .Text(" as owner of organisation ")
            .Link(Name, $"/organisation/{OrganisationId}")
            .Text(".");
    }
}