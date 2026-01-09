using System;
using Marten.Events.Projections;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.OrganisationView
{
    public class OrganisationReviewProjection : ViewProjection<Review, Guid>
    {
        public OrganisationReviewProjection()
        {
            ProjectEvent<OrganisationReviewAdded>(Project);
            ProjectEvent<OrganisationReviewRemoved>(Project);
            ProjectEvent<OrganisationReviewNameChanged>(Project);
            ProjectEvent<OrganisationReviewDescriptionChanged>(Project);
            ProjectEvent<OrganisationReviewRated>(Project);

            ProjectEvent<OrganisationReviewCommunityChanged>(Project);
            ProjectEvent<OrganisationReviewCenterChanged>(Project);
            ProjectEvent<OrganisationReviewHealthcareProviderChanged>(Project);

            ProjectEvent<OrganisationReviewReported>(Project);
        }

        private void Project(Review review, OrganisationReviewAdded @event)
        {
            review.Id = (Guid) @event.OrganisationReviewId;
            review.OrganisationId = (Guid) @event.OrganisationId;
            review.UserId = (Guid) @event.UserId;
            review.Visited = @event.Visited;
            review.ExperienceId = @event.ExperienceId;
            review.Name = @event.Name;
            review.Description = @event.Description;
            review.Rating = (int)@event.Rating;

            review.Community = @event.Community;
            review.Center = @event.Center;
            review.HealthcareProvider = @event.HealthcareProvider;

            review.Created = @event.EventTimestamp;
            review.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Review review, OrganisationReviewRemoved @event)
        {
            review.Removed = true;
            review.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Review review, OrganisationReviewNameChanged @event)
        {
            review.Name = @event.Name;
            review.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Review review, OrganisationReviewDescriptionChanged @event)
        {
            review.Description = @event.Description;
            review.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Review review, OrganisationReviewRated @event)
        {
            review.Rating = (int)@event.Rating;
            review.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Review review, OrganisationReviewCommunityChanged @event)
        {
            review.Community = @event.Review;
            review.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Review review, OrganisationReviewCenterChanged @event)
        {
            review.Center = @event.Review;
            review.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Review review, OrganisationReviewHealthcareProviderChanged @event)
        {
            review.HealthcareProvider = @event.Review;
            review.LastUpdated = @event.EventTimestamp;
        }

        private void Project(Review review, OrganisationReviewReported @event)
        {
            review.Reports.Add(new ReviewReport((Guid)@event.UserId, @event.Reason));
            review.Created = @event.EventTimestamp;
            review.LastUpdated = @event.EventTimestamp;
        }
    }
}