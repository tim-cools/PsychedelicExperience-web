using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Documents.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class DocumentPublishedNotification : Notification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Guid DocumentId { get; set; }

        public DocumentPublishedNotification()
        {
        }

        public DocumentPublishedNotification(Guid userId, Context<DocumentPublished> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            DocumentId = (Guid)context.Event.DocumentId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" published your document ")
            .Link(Name, $"/document/{DocumentId}")
            .Text(".");
    }
}