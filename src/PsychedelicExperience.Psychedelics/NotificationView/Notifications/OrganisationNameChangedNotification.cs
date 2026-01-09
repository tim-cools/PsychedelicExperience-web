using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class OrganisationNameChangedNotification : Notification
    {
        public Guid OrganisationId { get; set; }
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }

        public OrganisationNameChangedNotification()
        {
        }

        public OrganisationNameChangedNotification(Guid userId, Context<OrganisationNameChanged> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            OrganisationId = (Guid)context.Event.OrganisationId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" changed the name of your organisation to ")
            .Link(Name, $"/organisation/{OrganisationId}")
            .Text(".");
    }
}