using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;
using Organisation = PsychedelicExperience.Psychedelics.OrganisationView.Organisation;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdates
{
    public class OrganisationUpdate : AggregateRoot
    {
        public UserId UserId { get; set; }
        public OrganisationId OrganisationId { get; set; }

        public OrganisationUpdatePrivacy Privacy { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }

        public void Handle(User user, AddOrganisationUpdate command)
        {
            Publish(new OrganisationUpdateAdded
            {
                OrganisationId = command.OrganisationId,
                UpdateId = command.UpdateId,
                UserId = new UserId(user.Id),
                Subject = command.Subject,
                Content = command.Content,
                Privacy = command.Privacy
            });
        }

        public void Apply(OrganisationUpdateAdded @event)
        {
            Id = (Guid)@event.UpdateId;
            OrganisationId = @event.OrganisationId;
            UserId = @event.UserId;

            Subject = @event.Subject;
            Content = @event.Content;
        }

        public void Handle(User user, RemoveOrganisationUpdate command)
        {
            Publish(new OrganisationUpdateRemoved
            {
                OrganisationId = OrganisationId,
                UpdateId = (OrganisationUpdateId)Id,
                UserId = new UserId(user.Id)
            });
        }

        public void Apply(OrganisationUpdateRemoved @event)
        {
        }

        public void Handle(User user, SetOrganisationUpdateSubject command)
        {
            Publish(new OrganisationUpdateSubjectChanged
            {
                OrganisationId = command.OrganisationId,
                UpdateId = command.UpdateId,
                UserId = new UserId(user.Id),
                Subject = command.Subject
            });
        }

        public void Apply(OrganisationUpdateSubjectChanged @event)
        {
            Id = (Guid)@event.UpdateId;
            OrganisationId = @event.OrganisationId;
            UserId = @event.UserId;

            Subject = @event.Subject;
        }

        public void Handle(User user, SetOrganisationUpdateContent command)
        {
            Publish(new OrganisationUpdateContentChanged
            {
                OrganisationId = command.OrganisationId,
                UpdateId = command.UpdateId,
                UserId = new UserId(user.Id),
                Content = command.Content
            });
        }

        public void Apply(OrganisationUpdateContentChanged @event)
        {
            Id = (Guid)@event.UpdateId;
            OrganisationId = @event.OrganisationId;
            UserId = @event.UserId;

            Content = @event.Content;
        }

        public void Handle(User user, SetOrganisationUpdatePrivacy command)
        {
            Publish(new OrganisationUpdatePrivacyChanged
            {
                OrganisationId = command.OrganisationId,
                UpdateId = command.UpdateId,
                UserId = new UserId(user.Id),
                Privacy = command.Privacy
            });
        }

        public void Apply(OrganisationUpdatePrivacyChanged @event)
        {
            Id = (Guid)@event.UpdateId;
            OrganisationId = @event.OrganisationId;
            UserId = @event.UserId;

            Privacy = (OrganisationUpdatePrivacy) @event.Privacy;
        }

        public void EnsureCanAdd(User user, Organisation organisation)
        {
            if (user == null || !user.IsAdministrator() && !organisation.IsOwner(user))
            {
                throw new BusinessException($"could not add organisation update!");
            }
        }

        public void EnsureCanEdit(User user, Organisation organisation)
        {
            EnsureOrganisation(organisation);

            if (user == null || !user.IsAdministrator() && !organisation.IsOwner(user))
            {
                throw new BusinessException($"{user?.Id} could not edit organisation update {Id}!");
            }
        }

        public void EnsureCanRemove(User user, Organisation organisation)
        {
            EnsureOrganisation(organisation);

            if (user == null || !user.IsAdministrator() && !organisation.IsOwner(user))
            {
                throw new BusinessException($"{user?.Id} could not remove organisation update {Id}!");
            }
        }

        private void EnsureOrganisation(Organisation organisation)
        {
            if (OrganisationId != null && organisation != null && OrganisationId.Value == organisation.Id) return;

            throw new BusinessException($@"Update {Id} does not belong to organisation {organisation?.Id}! ({OrganisationId})");
        }
    }

    public enum OrganisationUpdatePrivacy
    {
        MembersOnly,
        Public
    }
}