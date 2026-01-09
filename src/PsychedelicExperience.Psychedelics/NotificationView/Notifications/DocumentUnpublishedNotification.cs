using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Documents.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class DocumentUnpublishedNotification : Notification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Guid DocumentId { get; set; }

        public DocumentUnpublishedNotification()
        {
        }

        public DocumentUnpublishedNotification(Guid userId, Context<DocumentUnpublished> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            DocumentId = (Guid)context.Event.DocumentId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" unpublished your document ")
            .Link(Name, $"/document/{DocumentId}")
            .Text(".");
    }
}