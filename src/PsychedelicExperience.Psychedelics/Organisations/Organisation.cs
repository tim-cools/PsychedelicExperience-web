using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Psychedelics.Tags.Handlers;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.Organisations
{
    public class Organisation : AggregateRoot
    {
        public IList<Guid> Owners { get; } = new List<Guid>();

        public DateTime? Invited { get; set; }

        public bool Person { get; set; }
        public IList<string> Types { get; } = new List<string>();
        
        public Name Name { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }

        public HealthcareProvider HealthcareProvider { get; set; }
        public Community Community { get; set; }
        public Center Center { get; set; }
        public Practitioner Practitioner { get; set; }

        public IList<Contact> Contacts { get; } = new List<Contact>();
        public IList<Photo> Photos { get; } = new List<Photo>();
        public IList<Tag> Tags { get; } = new List<Tag>();

        public OrganisationInfo Info { get; set; }
        public OrganisationInfo Warning { get; set; }

        public IList<OrganisationRelation> Relations { get; set; } = new List<OrganisationRelation>();

        public void Handle(User user, AddOrganisation command)
        {
            ValidateType(command.Person, user, command.Types);

            Publish(new OrganisationAdded
            {
                OrganisationId = command.OrganisationId,
                UserId = new UserId(user.Id),
                Person = command.Person,
                Types = command.Types ?? new string[] {},
                Name = command.Name,
                Description = command.Description,
                Contacts = command.Contacts,
                Tags = command.Tags,
                Address = command.Address,
                Center = command.Center,
                Community = command.Community,
                HealthcareProvider = command.HealthcareProvider,
                Practitioner = command.Practitioner
            });
            
            var email = command.Contacts?.FirstOrDefault(contact => contact.Type == ContactTypes.EMail)?.Value;

            if (email != null && user.HasEmail(email))
            {
                Publish(new OrganisationOwnerAdded
                {
                    OrganisationId = (OrganisationId) Id,
                    UserId = new UserId(user.Id),
                    OwnerId = (UserId) user.Id,
                    OrganisationName = Name
                });
            }
        }

        private static void ValidateType(bool person, User user, params string[] types)
        {
            if (types == null) return;

            var editor = user.IsAtLeast(Roles.ContentManager);

            var subCategory = TagRepository.OrganisationCategory(person);
            var notSupported = types
                .Where(type => !TagRepository.IsInCategory(TagsDomain.OrganisationTypes, subCategory, type, editor))
                .ToArray();

            if (notSupported.Length > 0)
            {
                throw new BusinessException($"{subCategory} can't have types: {string.Join(',', notSupported)}");
            }
        }

        public void Apply(OrganisationAdded @event)
        {
            Id = (Guid)@event.OrganisationId;
            Name = @event.Name;
            Description = @event.Description;
            Person = @event.Person;

            if (@event.Types != null){
                Types.AddRange(@event.Types);
            }

            Center = @event.Center;
            Practitioner = @event.Practitioner;
            Community = @event.Community;
            HealthcareProvider = @event.HealthcareProvider;

            if (@event.EMail != null)
            {
                AddContact(new Messages.Organisations.Contact { Type = ContactTypes.EMail, Value = @event.EMail.Value });
            }
            if (@event.Website != null)
            {
                AddContact(new Messages.Organisations.Contact { Type = ContactTypes.WebSite, Value = @event.Website });
            }
            if (@event.Phone != null)
            {
                AddContact(new Messages.Organisations.Contact { Type = ContactTypes.Phone, Value = @event.Phone });
            }

            @event.Contacts?.Each(AddContact);

            @event.Tags?.Each(tag => Tags.Add(new Tag(tag)));
            Address = @event.Address;
        }

        private void AddContact(Messages.Organisations.Contact newContact)
        {
            Contacts.Add(new Contact(newContact.Type, newContact.Value));
        }

        public void Handle(User user, ChangeOrganisationAddress command)
        {
            Publish(new OrganisationAddressChanged
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Address = command.Address
            });
        }

        public void Apply(OrganisationAddressChanged @event)
        {
            Address = @event.Address;
        }

        public void Handle(User user, ChangeOrganisationPerson command)
        {
            Publish(new OrganisationPersonChanged
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Person = command.Person
            });
        }

        public void Apply(OrganisationPersonChanged @event)
        {
            Person = @event.Person;
            if (Person)
            {
                Center = null;
            }

            var allowedTypes = TagRepository.GetTypes(Person);
            foreach (var type in Types.ToArray())
            {
                var normalized = type.Normalize();
                if (allowedTypes.All(allowed => allowed.NormalizedName != normalized))
                {
                    Types.Remove(type);
                }
            }
        }

        public void Handle(User user, ChangeOrganisationName command)
        {
            Publish(new OrganisationNameChanged
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Name = command.Name
            });
        }

        public void Apply(OrganisationNameChanged @event)
        {
            Name = @event.Name;
        }

        public void Handle(User user, ChangeOrganisationDescription command)
        {
            Publish(new OrganisationDescriptionChanged
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Description = command.Description
            });
        }

        public void Apply(OrganisationDescriptionChanged @event)
        {
            Description = @event.Description;
        }

        public void Handle(User user, ChangeOrganisationCenter command)
        {
            Publish(new OrganisationCenterChanged
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Center = command.Center
            });
        }

        public void Apply(OrganisationCenterChanged @event)
        {
            Center = @event.Center;
        }

        public void Handle(User user, ChangeOrganisationPractitioner command)
        {
            Publish(new OrganisationPractitionerChanged
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Practitioner = command.Practitioner
            });
        }

        public void Apply(OrganisationPractitionerChanged @event)
        {
            Practitioner = @event.Practitioner;
        }

        public void Handle(User user, ChangeOrganisationCommunity command)
        {
            Publish(new OrganisationCommunityChanged
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Community = command.Community
            });
        }

        public void Apply(OrganisationCommunityChanged @event)
        {
            Community = @event.Community;
        }

        public void Handle(User user, ChangeOrganisationHealthcareProvider command)
        {
            Publish(new OrganisationHealthcareProviderChanged
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                HealthcareProvider = command.HealthcareProvider
            });
        }

        public void Apply(OrganisationHealthcareProviderChanged @event)
        {
            HealthcareProvider = @event.HealthcareProvider;
        }

        public void Handle(User user, SetOrganisationWarning command)
        {
            Publish(new OrganisationWarningSet
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Title = command.Title,
                Content = command.Content,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationWarningSet @event)
        {
            Warning = new OrganisationInfo(@event.Title, @event.Content);
        }

        public void Handle(User user, SetOrganisationInfo command)
        {
            Publish(new OrganisationInfoSet
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Title = command.Title,
                Content = command.Content,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationInfoSet @event)
        {
            Info = new OrganisationInfo(@event.Title, @event.Content);
        }

        public void Handle(User user, AddOrganisationContact command)
        {
            var contact = GetContact(command.Type, command.Value);
            if (contact != null) return;

            Publish(new OrganisationContactAddded
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Type = command.Type,
                Value = command.Value,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationContactAddded @event)
        {
            Contacts.Add(new Contact(@event.Type, @event.Value));
        }

        public void Handle(User user, RemoveOrganisationContact command)
        {
            var contact = GetContact(command.Type, command.Value);
            if (contact == null) return;

            Publish(new OrganisationContactRemoved
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Type = contact.Type,
                Value = contact.Value,
                OrganisationName = Name
            });
        }

        private Contact GetContact(string type, string value)
        {
            return Contacts.FirstOrDefault(where => where.Type == type && where.Value == value);
        }

        public void Apply(OrganisationContactRemoved @event)
        {
            var contact = GetContact(@event.Type, @event.Value);

            if (contact != null)
            {
                Contacts.Remove(contact);
            }
        }

        public void Handle(User user, RemoveOrganisationWarning command)
        {
            Publish(new OrganisationWarningRemoved
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationWarningRemoved @event)
        {
            Warning = null;
        }

        public void Handle(User user, RemoveOrganisationInfo command)
        {
            Publish(new OrganisationInfoRemoved
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationInfoRemoved @event)
        {
            Info = null;
        }

        public void Handle(User user, RemoveOrganisation command)
        {
            Publish(new OrganisationRemoved
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationRemoved @event)
        {
        }

        public void Handle(User user, AddOrganisationTag command)
        {
            if (Tags.Any(criteria => criteria.Name == command.TagName)) return;

            Publish(new OrganisationTagAdded
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                TagName = command.TagName
            });
        }
        
        public void Apply(OrganisationTagAdded @event)
        {
            if (Tags.All(where => where.Name != @event.TagName))
            {
                var tag = new Tag(@event.TagName);
                Tags.Add(tag);
            }
        }

        public void Handle(User user, RemoveOrganisationTag command)
        {
            if (Tags.All(criteria => criteria.Name != command.TagName)) return;

            Publish(new OrganisationTagRemoved
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                TagName = command.TagName
            });
        }

        public void Apply(OrganisationTagRemoved @event)
        {
            Tags.Where(where => where.Name == @event.TagName)
                .ToList()
                .Each(tag => Tags.Remove(tag));
        }
        
        public void Handle(User user, AddOrganisationType command)
        {
            if (Types.Any(criteria => criteria == command.Type)) return;
            ValidateType(Person, user, command.Type);

            Publish(new OrganisationTypeAdded
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Type = command.Type,
                OrganisationName = Name
            });
        }
        
        public void Apply(OrganisationTypeAdded @event)
        {
            if (Types.All(where => where != @event.Type))
            {
                Types.Add(@event.Type);
            }
        }

        public void Handle(User user, RemoveOrganisationType command)
        {
            if (Types.All(criteria => criteria != command.Type)) return;

            Publish(new OrganisationTypeRemoved
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                Type = command.Type,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationTypeRemoved @event)
        {
            Types.Where(where => where == @event.Type)
                .ToList()
                .Each(tag => Types.Remove(tag));
        }

        public void Handle(User user, LinkOrganisation command)
        {
            Publish(new OrganisationLinked
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                TargetId = command.TargetId,
                Relation = command.Relation,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationLinked @event)
        {
            Relations.Add(new OrganisationRelation
            {
                Name = @event.Relation,
                Target = @event.TargetId
            });
        }

        public void Handle(User user, UnlinkOrganisation command)
        {
            Publish(new OrganisationUnlinked
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                TargetId = command.TargetId,
                Relation = command.Relation,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationUnlinked @event)
        {
            Relations.Where(where => Equals(@where.Target, @event.TargetId) && where.Name == @event.Relation)
                .ToList()
                .Each(tag => Relations.Remove(tag));
        }

        public void Handle(User user, InvitOrganisationOwner command)
        {
            Publish(new OrganisationOwnerInvited
            {
                OrganisationId = (OrganisationId)Id,
                UserId = command.UserId,
                UserEmail = command.UserEmail
            });
        }

        public void Apply(OrganisationOwnerInvited @event)
        {
            Invited = @event.EventTimestamp;
        }

        public void Handle(User currentUser, ConfirmOrganisationOwner command)
        {
            Publish(new OrganisationOwnerConfirmed
            {
                OrganisationId = (OrganisationId)Id,
                UserId = command.UserId
            });
        }

        public void Apply(OrganisationOwnerConfirmed @event)
        {
            Owners.Add(@event.UserId.Value);
        }

        public void Handle(User user, AddOrganisationOwner contextCommand, User owner)
        {
            if (contextCommand == null) throw new ArgumentNullException(nameof(contextCommand));
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            if (Owners.Any(criteria => criteria == owner.Id)) return;

            Publish(new OrganisationOwnerAdded
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                OwnerId = (UserId)owner.Id,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationOwnerAdded @event)
        {
            Owners.Add(@event.OwnerId.Value);
        }

        public void Handle(User user, RemoveOrganisationOwner command)
        {
            if (Owners.All(criteria => criteria != (Guid)command.OwnerId)) return;

            Publish(new OrganisationOwnerRemoved
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                OwnerId = command.OwnerId,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationOwnerRemoved @event)
        {
            Owners.Remove(@event.OwnerId.Value);
        }

        public void Handle(User user, ReportOrganisation command)
        {
            Publish(new OrganisationReported
            {
                OrganisationId = (OrganisationId)Id,
                OrganisationName = Name,
                UserId = new UserId(user.Id),
                Reason = command.Reason
            });
        }

        public void Apply(OrganisationReported @event)
        {
        }

        public void Handle(User user, AddOrganisationPhotos command)
        {
            Publish(new OrganisationPhotosAdded
            {
                OrganisationId = (OrganisationId)Id,
                UserId = command.UserId,
                Photos = command.Photos,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationPhotosAdded @event)
        {
            Photos.AddRange(@event.Photos);
        }

        public void Handle(User user, RemoveOrganisationPhoto command)
        {
            if (Photos.All(photo => !Equals(photo.Id, command.PhotoId))) return;

            Publish(new OrganisationPhotoRemoved
            {
                OrganisationId = (OrganisationId)Id,
                UserId = new UserId(user.Id),
                PhotoId = command.PhotoId,
                OrganisationName = Name
            });
        }

        public void Apply(OrganisationPhotoRemoved @event)
        {
            var photo = Photos.SingleOrDefault(where => Equals(where.Id, @event.PhotoId));
            Photos.Remove(photo);
        }

        public void Handle(User user, ContactOrganisation command, Organisation organisation)
        {
            var contact = organisation.Contacts.FirstOrDefault(where => where.Type == ContactTypes.EMail);
            if (contact == null)
            {
                throw new InvalidOperationException("Organisation has not email: " + organisation.Id);
            }

            var email = user != null && command.Email == null ? user.Email : command.Email;
            var userName = user?.DisplayName;

            Publish(new OrganisationContacted
            {
                OrganisationId = (OrganisationId)Id,
                UserId = command.UserId,
                EMail = email,
                UserName = userName,
                Subject = command.Subject,
                Message = command.Message,
                OrganisationEmail = contact.Value
            });
        }

        public void Apply(OrganisationContacted @event)
        {
        }

        public bool IsOwner(User user)
        {
            return user != null && Owners.Contains(user.Id);
        }

        public void EnsureCanAdd(User user)
        {
            if (user == null)
            {
                throw new BusinessException($"null could not add organisation!");
            }
        }

        public void EnsureCanReport(User user)
        {
            if (user == null)
            {
                throw new BusinessException($"null could not report organisation!");
            }
        }

        public void EnsureCanEdit(User user)
        {
            if (user == null || !user.IsAtLeast(Roles.ContentManager) && !IsOwner(user))
            {
                throw new BusinessException($"{user?.Id} could not edit organisation {Id}!");
            }
        }

        public void EnsureCanAdmin(User user)
        {
            if (!user.IsAdministrator())
            {
                throw new BusinessException($"{user.Id} could not admin organisation {Id}!");
            }
        }

        public void EnsureCanRemove(User user)
        {
            if (!user.IsAtLeast(Roles.ContentManager))
            {
                throw new BusinessException($"{user.Id} could not remove organisation {Id}!");
            }
        }

        public void EnsureCanAddOwner(User user, User newOwner)
        {
            if (user == null || !user.IsAdministrator())
            {
                throw new BusinessException($"'{user?.Id}' could not add owner to organisation {Id}!");
            }

            if (newOwner == null || newOwner.IsDisabled())
            {
                throw new BusinessException($"'{newOwner?.Id}' could not be added as owner to organisation {Id}!");
            }
        }
    }

    public class Contact
    {
        public string Type { get; private set; }
        public string Value { get; private set; }

        public Contact(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(Value)}: {Value}";
        }
    }

    public class OrganisationInfo
    {
        public string Title { get; }
        public string Content { get; }

        public OrganisationInfo(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }

    public class OrganisationRelation
    {
        public OrganisationId Target { get; set; }
        public string Name { get; set; }
    }
}