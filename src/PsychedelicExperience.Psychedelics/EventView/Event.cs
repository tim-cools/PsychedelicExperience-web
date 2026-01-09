using System;
using System.Collections.Generic;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.EventView
{
    public class Event
    {
        private Members _members = new Members();
        public Guid Id { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }
        public Guid LastUpdatedUserId { get; set; }

        public EventPrivacy Privacy { get; set; }
        public EventType EventType { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? LastUpdated { get; set; }

        public IList<string> Tags { get; } = new List<string>();
        public IList<string> TagsNormalized { get; } = new List<string>();

        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Description { get; set; }
        public Image Image { get; set; }

        public string LocationName { get; set; }
        public EventAddress Address { get; set; }
        public string Country { get; set; }

        public int Events { get; set; }
        public string SearchString { get; set; }

        public bool Removed { get; set; }

        public Members Members
        {
            get => _members ?? (_members = new Members());
            set => _members = value;
        }

        public bool CanEdit(User user, bool isOwner)
        {
            return isOwner || user != null && user.IsAtLeast(Roles.ContentManager);
        }

        public bool CanView(User user, bool isOrganisationMember, bool isOwner)
        {
            return Privacy == EventPrivacy.Public 
                || user != null && user.IsAtLeast(Roles.ContentManager)
                || Privacy == EventPrivacy.MembersOnly && (isOrganisationMember || isOwner);
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

        public void Updated(Common.Aggregates.Event @event, UserId userId)
        {
            Events++;

            LastUpdatedUserId = userId.Value;
            LastUpdated = @event.EventTimestamp;

            UpdateSearchField();
        }

        private void UpdateSearchField()
        {
            SearchString = $"{LocationName.NormalizeForSearch()} {Name.NormalizeForSearch()} {Address?.Name.NormalizeForSearch()} {Description.NormalizeForSearch()} ";
        }

        public string Slug() => Name.NormalizeForUrl();
    }

    public class Members
    {
        public int Invited { get; set; }
        public int Interested { get; set; }
        public int Attending { get; set; }
        public int NotAttending { get; set; }

        public void Add(Messages.Events.EventMemberStatus eventStatus)
        {
            switch (eventStatus)
            {
                case Messages.Events.EventMemberStatus.Invited:
                    Invited++;
                    break;
                case Messages.Events.EventMemberStatus.Attending:
                    Attending++;
                    break;
                case Messages.Events.EventMemberStatus.NotAttending:
                    NotAttending++;
                    break;
                case Messages.Events.EventMemberStatus.Interested:
                    Interested++;
                    break;
            }
        }

        public void Remove(Messages.Events.EventMemberStatus eventStatus)
        {
            switch (eventStatus)
            {
                case Messages.Events.EventMemberStatus.Invited:
                    Invited--;
                    break;
                case Messages.Events.EventMemberStatus.Attending:
                    Attending--;
                    break;
                case Messages.Events.EventMemberStatus.NotAttending:
                    NotAttending--;
                    break;
                case Messages.Events.EventMemberStatus.Interested:
                    Interested--;
                    break;
            }
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

    public enum EventType
    {
        Ceremony,
        Community,
        Conference
    }

    public enum EventPrivacy
    {
        Public,
        MembersOnly
    }


    public class EventAddress
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public string PlaceId { get; set; }
        public string Country { get; set; }
    }
}