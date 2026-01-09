using System;
using System.Collections.Generic;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Users.Domain;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.DocumentView
{
    public class Document
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LastUpdatedUserId { get; set; }

        public DocumentType DocumentType { get; set; }
        public DocumentStatus Status { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? LastUpdated { get; set; }

        public IList<string> Tags { get; } = new List<string>();
        public IList<string> TagsNormalized { get; } = new List<string>();
        
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string ExternalDescription { get; set; }
        public Image Image { get; set; }
        public bool Removed { get; set; }

        public int Events { get; set; }
        public string SearchString { get; set; }

        public bool CanEdit(User user)
        {
            return user != null && user.IsAtLeast(Roles.ContentManager);
        }

        public bool CanView(User user)
        {
            return Status == DocumentStatus.Published
                || CanEdit(user);
        }

        public void SetImage(Guid photoId, string fileName, string originalFileName)
        {
            Image = new Image(photoId, fileName, originalFileName);
        }

        public void ClearImage()
        {
            Image = null;
        }

        public void AddTag(string tagName)
        {
            Tags.Add(tagName);
            TagsNormalized.Add(tagName.NormalizeForSearch());
        }

        public void RemoveTag(string tagName)
        {
            Tags.Remove(tagName);
            TagsNormalized.Remove(tagName.NormalizeForSearch());
        }

        public void Updated(Event @event, UserId userId)
        {
            Events++;

            LastUpdatedUserId = userId.Value;
            LastUpdated = @event.EventTimestamp;

            UpdateSearchField();
        }

        private void UpdateSearchField()
        {
            SearchString = $"{Name.NormalizeForSearch()} " +
                           $"{Content.NormalizeForSearch()} ";
        }
    }

    public class Image
    {
        public Guid PhotoId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }

        public Image(Guid photoId, string fileName, string originalFileName)
        {
            PhotoId = photoId;
            FileName = fileName;
            OriginalFileName = originalFileName;
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
}