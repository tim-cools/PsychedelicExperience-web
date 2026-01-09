using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class OrganisationUpdateAddedNotification : Notification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Guid UpdateId { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        public OrganisationUpdateAddedNotification()
        {
        }

        public OrganisationUpdateAddedNotification(Guid userId, Context<OrganisationUpdateAdded> context, NotificationTopicStatus parent)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            UpdateId = (Guid)context.Event.UpdateId;
            OrganisationName = parent.Name;
            OrganisationId = (Guid)context.Event.OrganisationId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" added an update ")
            .Link(Name, $"/organisation/{OrganisationId}/update/{UpdateId}")
            .Text(" for your organisation ")
            .Link(OrganisationName, $"/organisation/{OrganisationId}")
            .Text(".");
    }
}