using System;
using Marten.Events.Projections;
using PsychedelicExperience.Psychedelics.Messages.Documents.Events;

namespace PsychedelicExperience.Psychedelics.DocumentView
{
    public class DocumentProjection : ViewProjection<Document, Guid>
    {
        public DocumentProjection()
        {
            ProjectEvent<DocumentAdded>(Project);
            ProjectEvent<DocumentRemoved>(Project);
            ProjectEvent<DocumentNameChanged>(Project);
            ProjectEvent<DocumentDescriptionChanged>(Project);
            ProjectEvent<DocumentSlugChanged>(Project);
            ProjectEvent<DocumentContentChanged>(Project);

            ProjectEvent<DocumentImageChanged>(Project);
            ProjectEvent<DocumentImageCleared>(Project);

            ProjectEvent<DocumentPublished>(Project);
            ProjectEvent<DocumentUnpublished>(Project);
       
            ProjectEvent<DocumentTagAdded>(Project);
            ProjectEvent<DocumentTagRemoved>(Project);
        }

        private void Project(Document view, DocumentRemoved @event)
        {
            view.Removed = true;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentSlugChanged @event)
        {
            view.Slug = @event.Slug?.Value;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentImageChanged @event)
        {
            view.SetImage(@event.Image.Id.Value, @event.Image.FileName, @event.Image.OriginalFileName);
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentImageCleared @event)
        {
            view.ClearImage();
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentContentChanged @event)
        {
            view.Content = @event.Content?.Value;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentPublished @event)
        {
            view.Status = DocumentStatus.Published;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentUnpublished @event)
        {
            view.Status = DocumentStatus.Draft;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentAdded @event)
        {
            view.Created = @event.EventTimestamp;
            view.Status = DocumentStatus.Draft;
            view.UserId = @event.UserId.Value;
            view.DocumentType = (DocumentType)@event.DocumentType;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentNameChanged @event)
        {
            view.Name = @event.Name?.Value;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentDescriptionChanged @event)
        {
            view.ExternalDescription = @event.Description?.Value;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentTagAdded @event)
        {
            view.AddTag(@event.TagName);
            view.Updated(@event, @event.UserId);
        }

        private void Project(Document view, DocumentTagRemoved @event)
        {
            view.RemoveTag(@event.TagName);
            view.Updated(@event, @event.UserId);
        }
    }
}