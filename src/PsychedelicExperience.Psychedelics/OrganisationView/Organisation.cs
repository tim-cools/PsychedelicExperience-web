using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.OrganisationView
{
    public class Organisation
    {
        public Guid Id { get; set; }
        public IList<Guid> Owners { get; } = new List<Guid>();

        public bool Person { get; set; }
        public IList<string> Types { get; } = new List<string>();

        public string Name { get; set; }
        public OrganisationAddress Address { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }

        public IList<Contact> Contacts { get; set; } = new List<Contact>();

        public OrganisationInfo Info { get; set; }
        public OrganisationInfo Warning { get; set; }
        public string SearchString { get; set; }

        public bool Removed { get; set; }

        public ReviewSummary Reviews { get; } = new ReviewSummary();

        public IList<Report> Reports { get; } = new List<Report>();
        public IList<Photo> Photos { get; } = new List<Photo>();
        public int PhotosCount { get; set; }

        public IList<Location> Locations { get; } = new List<Location>();

        public IList<Relation> Relations { get; } = new List<Relation>();
        public IList<Relation> RelationsFrom { get; } = new List<Relation>();

        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }

        public IList<string> Tags { get; } = new List<string>();
        public IList<string> TagsNormalized { get; } = new List<string>();

        public HealthcareProvider HealthcareProvider { get; set; }
        public Community Community { get; set; }
        public Center Center { get; set; }
        public DateTime? Invited { get; set; }
        public Practitioner Practitioner { get; set; }

        public bool IsOwner(User user)
        {
            return user != null && Owners.Contains(user.Id);
        }

        public Photo GetPhoto(Guid photoId)
        {
            var photo = Photos.FirstOrDefault(where => where.PhotoId == photoId);
            if (photo == null)
            {
                throw new InvalidOperationException("Origanisation Photo not found:" + photoId);
            }
            return photo;
        }

        public Location GetLocation(Guid locationId)
        {
            var location = Locations.FirstOrDefault(where => where.Id == locationId);
            if (location == null)
            {
                throw new InvalidOperationException("Location not found:" + locationId);
            }
            return location;
        }

        public bool CanView(User user)
        {
            return (user != null && user.IsAdministrator()) || !Removed;
        }

        public void AddTag(string tagName)
        {
            if (!Tags.Contains(tagName))
            {
                Tags.Add(tagName);
                TagsNormalized.Add(tagName.NormalizeForSearch());
            }
        }

        public void RemoveTag(string tagName)
        {
            while (Tags.Contains(tagName))
            {
                Tags.Remove(tagName);
                TagsNormalized.Remove(tagName.NormalizeForSearch());
            }
        }

        public void AddType(string type)
        {
            if (!Types.Contains(type))
            {
                Types.Add(type);
            }
        }

        public void RemoveType(string type)
        {
            while (Types.Contains(type))
            {
                Types.Remove(type);
            }
        }

        public void Link(OrganisationId targetId, string name)
        {
            if (targetId == null) throw new ArgumentNullException(nameof(targetId));

            Relations.Add(new Relation { Name = name, TargetId = targetId.Value});
        }

        public void Unlink(OrganisationId targetId, string name)
        {
            if (targetId == null) throw new ArgumentNullException(nameof(targetId));

            Relations
                .Where(relation => relation.Name == name && relation.TargetId == targetId.Value)
                .ToList()
                .Each(relation => Relations.Remove(relation));
        }

        public void LinkFrom(OrganisationId targetId, string name)
        {
            if (targetId == null) throw new ArgumentNullException(nameof(targetId));

            RelationsFrom.Add(new Relation { Name = name, TargetId = targetId.Value});
        }

        public void UnlinkFrom(OrganisationId targetId, string name)
        {
            if (targetId == null) throw new ArgumentNullException(nameof(targetId));

            RelationsFrom
                .Where(relation => relation.Name == name && relation.TargetId == targetId.Value)
                .ToList()
                .Each(relation => RelationsFrom.Remove(relation));
        }

        public void AddOwner(UserId userId)
        {
            Owners.Add(userId.Value);
        }

        public void RemoveOwner(UserId userId)
        {
            Owners.Remove(userId.Value);
        }

        public void AddPhoto(Guid photoId, string fileName, string originalFileName)
        {
            Photos.Add(new Photo(photoId, fileName, originalFileName));
            PhotosCount++;
        }

        public void RemovePhoto(Guid photoId)
        {
            var photo = Photos.Single(where => where.PhotoId == photoId);
            Photos.Remove(photo);
            PhotosCount--;
        }

        public void AddContact(string type, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            var newContact = new Contact
            {
                Value = value,
                Type = type
            };
            Contacts.Add(newContact);
        }

        
        public void RemoveContact(string type, string value)
        {
            var contacts = Contacts
                .Where(where => where.Type == type && where.Value == value)
                .ToArray();

            contacts.Each(contact => Contacts.Remove(contact));
        }

        public string Slug()
        {
            return Name.NormalizeForUrl();
        }

        public string GetUrl()
        {
            return $"/organisation/{new ShortGuid(Id)}/{Slug()}";
        }

        public Guid[] RelationOrganisationIds()
        {
            var ids = (Relations ?? new List<Relation>())
                .Union(RelationsFrom ?? new Relation[0]);

            return ids
                .Select(relation => relation.TargetId)
                .ToArray();
        }
    }

    public class Contact
    {
        public string Type { get; set; }
        public string Value { get; set; }        
    }

    public class OrganisationAddress
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public string PlaceId { get; set; }
        public string Country { get; set; }
    }

    public class ReviewSummary
    {
        public int Count { get; set; }
        public int Rating { get; set; }
    }

    public class Review
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }
        public ExperienceId ExperienceId { get; set; }

        public DateTime? Visited { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int Rating { get; set; }
        public IList<ReviewReport> Reports { get; } = new List<ReviewReport>();

        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }

        public bool Removed { get; set; }

        public CommunityReview Community { get; set; }
        public CenterReview Center { get; set; }
        public HealthcareProviderReview HealthcareProvider { get; set; }

        public string Slug()
        {
            return Name.NormalizeForUrl();
        }

        public bool IsOwner(User user)
        {
            return user != null && UserId == user.Id;
        }

        public string GetUrl()
        {
            return $"/organisation/{new ShortGuid(OrganisationId)}/review/{new ShortGuid(Id)}/{Slug()}";
        }
    }

    public class Report
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; }
        public DateTime DateTime { get; set; }

        public Report(Guid userId, string reason, DateTime dateTime)
        {
            UserId = userId;
            Reason = reason;
            DateTime = dateTime;
        }
    }

    public class OrganisationInfo
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class ReviewReport
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; }

        public ReviewReport(Guid userId, string reason)
        {
            UserId = userId;
            Reason = reason;
        }
    }

    public class Photo
    {
        public Guid PhotoId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }

        public Photo(Guid photoId, string fileName, string originalFileName)
        {
            PhotoId = photoId;
            FileName = fileName;
            OriginalFileName = originalFileName;
        }
    }

    public class Location
    {
        public Guid Id { get; set; }

        public IList<Guid> Owners { get; } = new List<Guid>();

        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string EMail { get; set; }
        public string Phone { get; set; }
        public string WebSite { get; set; }
        public bool Removed { get; set; }

        public IList<Report> Reports { get; } = new List<Report>();
        public IList<Photo> Photos { get; } = new List<Photo>();

        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }

        public IList<string> Tags { get; } = new List<string>();
        public IList<string> TagsNormalized { get; } = new List<string>();

        public Location(Guid id, Guid userId, string name)
        {
            Id = id;
            Owners.Add(userId);
            Name = name;
        }
        public Photo GetPhoto(Guid photoId)
        {
            var photo = Photos.FirstOrDefault(where => where.PhotoId == photoId);
            if (photo == null)
            {
                throw new InvalidOperationException("Location Photo not found:" + photoId);
            }
            return photo;
        }

        public void AddTag(string tagName)
        {
            Tags.Add(tagName);
            TagsNormalized.Add(tagName.NormalizeForSearch());
        }

        public void RemoveTag(string tagName)
        {
            Tags.Remove(tagName);
            Tags.Remove(tagName.NormalizeForSearch());
        }

        public void AddPhoto(Guid photoId, string fileName, string originalFileName)
        {
            Photos.Add(new Photo(photoId, fileName, originalFileName));
        }

        public void RemovePhoto(Guid photoId)
        {
            var photo = Photos.Single(where => where.PhotoId == photoId);
            Photos.Add(photo);
        }
    }

    public class Relation
    {
        public Guid TargetId { get; set; }
        public string Name { get; set; }
    }
}