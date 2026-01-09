using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Commands;
using PsychedelicExperience.Psychedelics.Messages.Documents.Events;
using Image = PsychedelicExperience.Psychedelics.Messages.Documents.Image;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.Documents
{
    public class Document : AggregateRoot
    {
        public DocumentStatus DocumentStatus { get; set; }
        public DocumentType DocumentType { get; set; }

        public UserId UserId { get; set; }

        public Name Name { get; set; }
        public Description Description { get; set; }
        public Description Content { get; set; }
        public Name Slug { get; set; }
        public Image Image { get; set; }

        public IList<Tag> Tags { get; } = new List<Tag>();

        public void Handle(User user, AddDocument command)
        {
            Publish(new DocumentAdded
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id),
                DocumentType = command.DocumentType
            });
        }

        public void Handle(User user, AddDocumentTag command)
        {
            if (Tags.Any(criteria => criteria.Name == command.TagName)) return;

            Publish(new DocumentTagAdded
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id),
                TagName = command.TagName
            });
        }

        public void Handle(User user, ChangeDocumentContent command)
        {
            Publish(new DocumentContentChanged
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id),
                Content = command.Content
            });
        }

        public void Handle(User user, ChangeDocumentDescription command)
        {
            Publish(new DocumentDescriptionChanged
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id),
                Description = command.Description
            });
        }

        public void Handle(User user, ChangeDocumentImage command)
        {
            Publish(new DocumentImageChanged
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id),
                Image = command.Image
            });
        }

        public void Handle(User user, ChangeDocumentName command)
        {
            Publish(new DocumentNameChanged
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id),
                Name = command.Name
            });
        }

        public void Handle(User user, ChangeDocumentSlug command)
        {
            Publish(new DocumentSlugChanged
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id),
                Slug = command.Slug
            });
        }

        public void Handle(User user, ClearDocumentImage command)
        {
            if (Image == null) return;

            Publish(new DocumentImageCleared
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id)
            });
        }

        public void Handle(User user, PublishDocument command)
        {
            if (DocumentStatus != DocumentStatus.Draft) return;

            Publish(new DocumentPublished
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id)
            });
        }

        public void Handle(User user, RemoveDocument command)
        {
            Publish(new DocumentRemoved
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id)
            });
        }

        public void Handle(User user, RemoveDocumentTag command)
        {
            if (Tags.All(criteria => criteria.Name != command.TagName)) return;

            Publish(new DocumentTagRemoved
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id),
                TagName = command.TagName
            });
        }

        public void Handle(User user, UnpublishDocument command)
        {
            if (DocumentStatus != DocumentStatus.Published) return;

            Publish(new DocumentUnpublished
            {
                DocumentId = (DocumentId)Id,
                UserId = new UserId(user.Id)
            });
        }

        public void Apply(DocumentAdded @event)
        {
            DocumentStatus = DocumentStatus.Draft;
            DocumentType = (DocumentType) @event.DocumentType;
            UserId = @event.UserId;
        }

        public void Apply(DocumentRemoved @event)
        {
        }

        public void Apply(DocumentNameChanged @event)
        {
            Name = @event.Name;
        }

        public void Apply(DocumentDescriptionChanged @event)
        {
            Description = @event.Description;
        }

        public void Apply(DocumentContentChanged @event)
        {
            Content = @event.Content;
        }

        public void Apply(DocumentSlugChanged @event)
        {
            Slug = @event.Slug;
        }

        public void Apply(DocumentImageChanged @event)
        {
            Image = @event.Image;
        }


        public void Apply(DocumentImageCleared @event)
        {
            Image = null;
        }

        public void Apply(DocumentTagAdded @event)
        {
            var tag = new Tag(@event.TagName);
            Tags.Add(tag);
        }

        public void Apply(DocumentTagRemoved @event)
        {
            var tag = Tags.Single(where => Equals(where.Name, @event.TagName));
            Tags.Remove(tag);
        }

        public void Apply(DocumentPublished @event)
        {
            DocumentStatus = DocumentStatus.Published;
        }

        public void Apply(DocumentUnpublished @event)
        {
            DocumentStatus = DocumentStatus.Draft;
        }

        public void EnsureCanEdit(User user)
        {
            if (user == null || !user.IsAtLeast(Roles.ContentManager))
            {
                throw new BusinessException($"{user?.Id} could not edit document {Id}!");
            }
        }
    }


    public enum DocumentType
    {
        Page
    }

    public enum DocumentStatus
    {
        Draft,
        Published
    }

    public class Tag
    {
        public string Name { get; }

        public Tag(string name)
        {
            Name = name;
        }
    }
}