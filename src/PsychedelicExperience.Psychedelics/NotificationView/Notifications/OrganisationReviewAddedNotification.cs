using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class OrganisationReviewAddedNotification : Notification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Guid ReviewId { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        public OrganisationReviewAddedNotification()
        {
        }

        public OrganisationReviewAddedNotification(Guid userId, Context<OrganisationReviewAdded> context, NotificationTopicStatus parent)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            ReviewId = (Guid)context.Event.OrganisationReviewId;
            OrganisationName = parent.Name;
            OrganisationId = (Guid)context.Event.OrganisationId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" added a review ")
            .Link(Name, $"/organisation/{OrganisationId}/review/{ReviewId}")
            .Text(" for your organisation ")
            .Link(OrganisationName, $"/organisation/{OrganisationId}")
            .Text(".");
    }
}