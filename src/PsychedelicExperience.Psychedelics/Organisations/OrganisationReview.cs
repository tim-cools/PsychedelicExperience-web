using System;
using System.Collections.Generic;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.Organisations
{
    public class OrganisationReview : AggregateRoot
    {
        public IList<Guid> Owners { get; } = new List<Guid>();
        public OrganisationId OrganisationId { get; set; }

        public DateTime Visited { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ScaleOf5 Rating { get; set; }

        public CommunityReview Community { get; set; }
        public CenterReview Center { get; set; }
        public HealthcareProviderReview HealthcareProvider { get; set; }
        public ExperienceId ExperienceId { get; set; }

        public void Handle(User user, AddOrganisationReview command)
        {
            Publish(new OrganisationReviewAdded
            {
                OrganisationReviewId = command.OrganisationReviewId,
                UserId = new UserId(user.Id),
                OrganisationId = command.OrganisationId,
                Center = command.Center,
                Community = command.Community,
                HealthcareProvider = command.HealthcareProvider,
                Name = command.Name,
                Description = command.Description,
                Rating = command.Rating,
                Visited = command.Visited,
                ExperienceId = command.Experience?.ExperienceId,
                Feedback = command.Feedback
            });
        }

        public void Apply(OrganisationReviewAdded @event)
        {
            Id = (Guid)@event.OrganisationReviewId;
            Owners.Add((Guid)@event.UserId);
            OrganisationId = @event.OrganisationId;
            ExperienceId = @event.ExperienceId;
            Name = @event.Name;
            Description = @event.Description;
            Rating = @event.Rating;
            Community = @event.Community;
            Center = @event.Center;
            Visited = @event.Visited;
            HealthcareProvider = @event.HealthcareProvider;
        }

        public void Handle(User user, ChangeOrganisationReviewName command)
        {
            Publish(new OrganisationReviewNameChanged
            {
                OrganisationReviewId = (OrganisationReviewId)Id,
                UserId = new UserId(user.Id),
                Name = command.Name
            });
        }

        public void Apply(OrganisationReviewNameChanged @event)
        {
            Name = @event.Name;
        }

        public void Handle(User user, ChangeOrganisationReviewDescription command)
        {
            EnsureCanEdit(user);

            Publish(new OrganisationReviewDescriptionChanged
            {
                OrganisationReviewId = (OrganisationReviewId)Id,
                UserId = new UserId(user.Id),
                Description = command.Description
            });
        }

        public void Apply(OrganisationReviewDescriptionChanged @event)
        {
            Description = @event.Description;
        }

        public void Handle(User user, RateOrganisationReview command)
        {
            EnsureCanEdit(user);

            Publish(new OrganisationReviewRated
            {
                OrganisationId = OrganisationId,
                OrganisationReviewId = (OrganisationReviewId)Id,
                UserId = new UserId(user.Id),
                Rating = command.Rating,
                PreviousRating = Rating
            });
        }

        public void Apply(OrganisationReviewRated @event)
        {
            Rating = @event.Rating;
        }

        public void Handle(User user, ChangeOrganisationReviewCenter command)
        {
            EnsureCanEdit(user);

            Publish(new OrganisationReviewCenterChanged
            {
                OrganisationId = OrganisationId,
                OrganisationReviewId = (OrganisationReviewId)Id,
                UserId = new UserId(user.Id),
                Review = command.Review,
                Previous = Center
            });
        }

        public void Apply(OrganisationReviewCenterChanged @event)
        {
            Center = @event.Review;
        }


        public void Handle(User user, ChangeOrganisationReviewCommunity command)
        {
            EnsureCanEdit(user);

            Publish(new OrganisationReviewCommunityChanged
            {
                OrganisationId = OrganisationId,
                OrganisationReviewId = (OrganisationReviewId)Id,
                UserId = new UserId(user.Id),
                Review = command.Review,
                Previous = Community
            });
        }

        public void Apply(OrganisationReviewCommunityChanged @event)
        {
            Community = @event.Review;
        }

        public void Handle(User user, ChangeOrganisationReviewHealthcareProvider command)
        {
            EnsureCanEdit(user);

            Publish(new OrganisationReviewHealthcareProviderChanged
            {
                OrganisationId = OrganisationId,
                OrganisationReviewId = (OrganisationReviewId)Id,
                UserId = new UserId(user.Id),
                Review = command.Review,
                Previous = HealthcareProvider
            });
        }

        public void Apply(OrganisationReviewHealthcareProviderChanged @event)
        {
            HealthcareProvider = @event.Review;
        }

        public void Handle(User user, ReportOrganisationReview command)
        {
            EnsureCanEdit(user);

            Publish(new OrganisationReviewReported
            {
                OrganisationReviewId = (OrganisationReviewId)Id,
                UserId = new UserId(user.Id),
                Reason = command.Reason,
                OrganisationId = OrganisationId
            });
        }

        public void Apply(OrganisationReviewReported @event)
        {
        }

        public void Handle(User user, RemoveOrganisationReview command)
        {
            EnsureCanEdit(user);

            Publish(new OrganisationReviewRemoved
            {
                OrganisationId = OrganisationId,
                OrganisationReviewId = (OrganisationReviewId)Id,
                UserId = new UserId(user.Id),
                Rating = Rating
            });
        }

        public void Apply(OrganisationReviewRemoved @event)
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
                throw new BusinessException($"null could not add organisation review {Id}!");
            }
        }

        public void EnsureCanReport(User user)
        {
            if (user == null)
            {
                throw new BusinessException($"null could not report organisation review {Id}!");
            }
        }

        public void EnsureCanEdit(User user)
        {
            if (!user.IsAdministrator() && !IsOwner(user))
            {
                throw new BusinessException($"{user.Id} could not edit organisation review {Id}!");
            }
        }

        public void EnsureCanRemove(User user)
        {
            if (!user.IsAdministrator())
            {
                throw new BusinessException($"{user.Id} could not remove organisation review {Id}!");
            }
        }
    }
}