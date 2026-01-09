using System;
using System.Linq;
using Baseline;
using Marten;
using Marten.Events.Projections;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Tags.Handlers;

namespace PsychedelicExperience.Psychedelics.OrganisationView
{
    public class OrganisationProjection : ViewProjection<Organisation, Guid>
    {
        public OrganisationProjection()
        {
            ProjectEvent<OrganisationAdded>(Project);
            ProjectEvent<OrganisationNameChanged>(Project);
            ProjectEvent<OrganisationAddressChanged>(Project);
            ProjectEvent<OrganisationPersonChanged>(Project);
            ProjectEvent<OrganisationDescriptionChanged>(Project);

            ProjectEvent<OrganisationCenterChanged>(Project);
            ProjectEvent<OrganisationPractitionerChanged>(Project);
            ProjectEvent<OrganisationCommunityChanged>(Project);
            ProjectEvent<OrganisationHealthcareProviderChanged>(Project);

            ProjectEvent<OrganisationContactAddded>(Project);
            ProjectEvent<OrganisationContactRemoved>(Project);

            ProjectEvent<OrganisationPhotosAdded>(Project);
            ProjectEvent<OrganisationPhotoRemoved>(Project);
            ProjectEvent<OrganisationRemoved>(Project);
            ProjectEvent<OrganisationReported>(Project);

            ProjectEvent<OrganisationTagAdded>(Project);
            ProjectEvent<OrganisationTagRemoved>(Project);

            ProjectEvent<OrganisationTypeAdded>(Project);
            ProjectEvent<OrganisationTypeRemoved>(Project);

            ProjectEvent<OrganisationLinked>(Project);
            ProjectEvent<OrganisationUnlinked>(Project);

            ProjectEvent<OrganisationOwnerAdded>(Project);
            ProjectEvent<OrganisationOwnerRemoved>(Project);
            ProjectEvent<OrganisationOwnerInvited>(Project);
            ProjectEvent<OrganisationOwnerConfirmed>(Project);

            ProjectEvent<OrganisationInfoSet>(Project);
            ProjectEvent<OrganisationInfoRemoved>(Project);
            ProjectEvent<OrganisationWarningSet>(Project);
            ProjectEvent<OrganisationWarningRemoved>(Project);

            ProjectEvent<OrganisationReviewAdded>(@event => (Guid)@event.OrganisationId, Project);
            ProjectEvent<OrganisationReviewRated>(@event => (Guid)@event.OrganisationId, Project);
            ProjectEvent<OrganisationReviewRemoved>(@event => (Guid)@event.OrganisationId, Project);
        }

        private void Project(Organisation view, OrganisationAdded @event)
        {
            view.Name = (string) @event.Name;
            view.Person = @event.Person;
            view.Types.AddMany(@event.Types);
            view.Description = @event.Description;

            view.AddContact(ContactTypes.EMail, (string) @event.EMail);
            view.AddContact(ContactTypes.Phone, @event.Phone);
            view.AddContact(ContactTypes.WebSite, @event.Website);

            if (@event.Contacts != null)
            {
                view.Contacts.AddRange(@event.Contacts.Select(Map));
            }

            view.Address = CreateAddress(@event.Address);
            view.Country = @event.Address?.Country;

            view.Center = @event.Center;
            view.Practitioner = @event.Practitioner;
            view.Community = @event.Community;
            view.HealthcareProvider = @event.HealthcareProvider;

            @event.Tags?.Each(view.AddTag);

            view.Created = @event.EventTimestamp;
            view.LastUpdated = @event.EventTimestamp;

            UpdateSearchField(view);
        }

        private Contact Map(Messages.Organisations.Contact contact)
        {
            return new Contact
            {
                Value = contact.Value,
                Type = contact.Type
            };
        }

        private static OrganisationAddress CreateAddress(Address address)
        {
            return address != null
                ? new OrganisationAddress
                {
                    Name = address.Name,
                    Position = MapPosition(address),
                    Country = address.Country.NormalizeForSearch(),
                    PlaceId = address.PlaceId
                }
                : null;
        }

        private static Position MapPosition(Address address)
        {
            return address.Location != null
                ? new Position
                {
                    Latitude = address.Location.Latitude,
                    Longitude = address.Location.Longitude
                }
                : null;
        }

        private void Project(Organisation view, OrganisationAddressChanged @event)
        {
            view.Address = CreateAddress(@event.Address);
            view.Country = @event.Address?.Country;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationPersonChanged @event)
        {
            view.Person = @event.Person;
            if (view.Person)
            {
                view.Center = null;
            }

            var allowedTypes = TagRepository.GetTypes(view.Person);
            foreach (var type in view.Types.ToArray())
            {
                var normalized = type.Normalize();
                if (allowedTypes.All(allowed => allowed.NormalizedName != normalized))
                {
                    view.Types.Remove(type);
                }
            }

            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationDescriptionChanged @event)
        {
            view.Description = @event.Description;
            view.LastUpdated = @event.EventTimestamp;

            UpdateSearchField(view);
        }

        private void Project(Organisation view, OrganisationCenterChanged @event)
        {
            view.Center = @event.Center;
            view.LastUpdated = @event.EventTimestamp;

            UpdateSearchField(view);
        }

        private void Project(Organisation view, OrganisationPractitionerChanged @event)
        {
            view.Practitioner = @event.Practitioner;
            view.LastUpdated = @event.EventTimestamp;

            UpdateSearchField(view);
        }

        private void Project(Organisation view, OrganisationHealthcareProviderChanged @event)
        {
            view.HealthcareProvider = @event.HealthcareProvider;
            view.LastUpdated = @event.EventTimestamp;

            UpdateSearchField(view);
        }

        
        private void Project(Organisation view, OrganisationCommunityChanged @event)
        {
            view.Community = @event.Community;
            view.LastUpdated = @event.EventTimestamp;

            UpdateSearchField(view);
        }

        private void Project(Organisation view, OrganisationNameChanged @event)
        {
            view.Name = (string)@event.Name;
            view.LastUpdated = @event.EventTimestamp;

            UpdateSearchField(view);
        }

        private void Project(Organisation view, OrganisationContactAddded @event)
        {
            view.AddContact(@event.Type, @event.Value);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationContactRemoved @event)
        {
            view.RemoveContact(@event.Type, @event.Value); 
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationPhotosAdded @event)
        {
            foreach (var photo in @event.Photos)
            {
                view.AddPhoto(photo.Id.Value, photo.FileName, photo.OriginalFileName);
            }

            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationPhotoRemoved @event)
        {
            view.RemovePhoto((Guid)@event.PhotoId);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationRemoved @event)
        {
            view.Removed = true;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationReported @event)
        {
            view.Reports.Add(new Report((Guid)@event.UserId, @event.Reason, @event.EventTimestamp));
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationReviewAdded @event)
        {
            view.Reviews.Count += 1;
            view.Reviews.Rating += (int)@event.Rating;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationReviewRated @event)
        {
            view.Reviews.Rating -= (int)@event.PreviousRating;
            view.Reviews.Rating += (int)@event.Rating;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationReviewRemoved @event)
        {
            view.Reviews.Count -= 1;
            view.Reviews.Rating -= (int)@event.Rating;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationTagAdded @event)
        {
            view.AddTag(@event.TagName);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationTagRemoved @event)
        {
            view.RemoveTag(@event.TagName);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationTypeAdded @event)
        {
            view.AddType(@event.Type);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationTypeRemoved @event)
        {
            view.RemoveType(@event.Type);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(IDocumentSession session, Organisation view, OrganisationLinked @event)
        {
            view.Link(@event.TargetId, @event.Relation);
            view.LastUpdated = @event.EventTimestamp;

            var target = session.Load<Organisation>(@event.TargetId);
            target.LinkFrom(@event.OrganisationId, @event.Relation);
            target.LastUpdated = @event.EventTimestamp;

            session.Store(target);
        }

        private void Project(IDocumentSession session, Organisation view, OrganisationUnlinked @event)
        {
            view.Unlink(@event.TargetId, @event.Relation);
            view.LastUpdated = @event.EventTimestamp;

            var target = session.Load<Organisation>(@event.TargetId);
            target.UnlinkFrom(@event.OrganisationId, @event.Relation);
            target.LastUpdated = @event.EventTimestamp;

            session.Store(target);
        }

        private void Project(Organisation view, OrganisationOwnerInvited @event)
        {
            view.Invited = @event.EventTimestamp;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationOwnerConfirmed @event)
        {
            view.AddOwner(@event.UserId);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationOwnerAdded @event)
        {
            view.AddOwner(@event.OwnerId);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationOwnerRemoved @event)
        {
            view.RemoveOwner(@event.OwnerId);
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Organisation view, OrganisationInfoSet @event)
        {
            view.Info = new OrganisationInfo
            {
                Title = @event.Title,
                Content = @event.Content
            };
        }

        private void Project(Organisation view, OrganisationInfoRemoved @event)
        {
            view.Info = null;
        }

        private void Project(Organisation view, OrganisationWarningSet @event)
        {
            view.Warning = new OrganisationInfo
            {
                Title = @event.Title,
                Content = @event.Content
            };
        }

        private void Project(Organisation view, OrganisationWarningRemoved @event)
        {
            view.Warning = null;
        }

        private void UpdateSearchField(Organisation organisation)
        {
            var searchString = $"{organisation.Name?.NormalizeForSearch()} " +
                               $"{organisation.Description?.NormalizeForSearch()} "+
                               $"{organisation.Address?.Name?.NormalizeForSearch()} " +
                               $"{organisation.Center?.NormalizeForSearch()} "+
                               $"{organisation.Practitioner?.NormalizeForSearch()} ";

            organisation.SearchString = searchString;
        }
    }
}