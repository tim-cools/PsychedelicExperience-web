using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class EventAddedNotification : Notification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Guid EventId { get; set; }
        public string OrganisationName { get; set; }
        public Guid OrganisationId { get; set; }

        public EventAddedNotification()
        {
        }

        public EventAddedNotification(Guid userId, Context<EventAdded> context, NotificationTopicStatus parentStatus)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            EventId = (Guid)context.Event.EventId;
            OrganisationName = parentStatus.Name;
            OrganisationId = (Guid)context.Event.OrganisationId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" added a public event  ")
            .Link(Name, $"/event/{EventId}")
            .Text(" for ")
            .Link(OrganisationName, $"/organisation/{OrganisationId}")
            .Text(".");
    }
}