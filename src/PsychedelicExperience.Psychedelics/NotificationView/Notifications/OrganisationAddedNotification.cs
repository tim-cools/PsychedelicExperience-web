using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class OrganisationAddedNotification : Notification
    {
        public Guid OrganisationId { get; set; }
        public Guid ChangedUserId { get; set; }
        public string Title { get; set; }

        public OrganisationAddedNotification()
        {
        }

        public OrganisationAddedNotification(Guid userId, Context<OrganisationAdded> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Title = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            OrganisationId = (Guid)context.Event.OrganisationId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" added an organisation ")
            .Link(Title, $"/organisation/{OrganisationId}")
            .Text(".");
    }
}