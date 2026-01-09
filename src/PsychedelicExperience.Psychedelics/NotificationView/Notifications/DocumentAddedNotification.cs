using System;
using Baseline;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Documents.Events;

namespace PsychedelicExperience.Psychedelics.NotificationView.Notifications
{
    public class DocumentAddedNotification : Notification
    {
        public Guid ChangedUserId { get; set; }
        public string Name { get; set; }
        public Guid DocumentId { get; set; }

        public DocumentAddedNotification()
        {
        }

        public DocumentAddedNotification(Guid userId, Context<DocumentAdded> context)
            : base(userId, context.Event.EventTimestamp)
        {
            Name = context.StatusView.Name;
            ChangedUserId = (Guid)context.Event.UserId;
            DocumentId = (Guid)context.Event.DocumentId;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(ChangedUserId, UserId, userInfoResolver)
            .Text(" added a document ")
            .Link(Name, $"/document/{DocumentId}")
            .Text(".");
    }

    public class RemovedNotification : Notification
    {
        public Guid RemovedUserId { get; set; }
        public Topic Topic { get; set; }
        public string Name { get; }

        protected RemovedNotification()
        {
        }

        public RemovedNotification(Guid userId, DateTime dateTime, Guid removedUserId, Topic topic, string name) : base(userId, dateTime)
        {
            RemovedUserId = removedUserId;
            Topic = topic;
            Name = name;
        }

        public override Markup Format(IUserInfoResolver userInfoResolver) => new Markup()
            .User(RemovedUserId, UserId, userInfoResolver)
            .Text($" removed {Topic.TopicType.ToString().SplitPascalCase()} '{Name}'.");
    }
}