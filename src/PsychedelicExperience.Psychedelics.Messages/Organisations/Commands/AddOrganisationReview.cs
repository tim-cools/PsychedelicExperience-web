using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class AddOrganisationReview : IRequest<Result>
    {
        public OrganisationReviewId OrganisationReviewId { get; }
        public OrganisationId OrganisationId { get; }

        public UserId UserId { get; }
        public DateTime Visited { get; }

        public string Name { get; }

        public string Description { get; }
        public ScaleOf5 Rating { get; }

        public string Feedback { get; }

        public CenterReview Center { get; }
        public CommunityReview Community { get; }
        public HealthcareProviderReview HealthcareProvider { get; }

        public Experience Experience { get; }


        public AddOrganisationReview(OrganisationReviewId organisationReviewId, DateTime visited,
            OrganisationId organisationId, UserId userId, string name, string description, ScaleOf5 rating,
            CenterReview center, CommunityReview community, HealthcareProviderReview healthcareProvider,
            Experience experience, string feedback)
        {
            OrganisationReviewId = organisationReviewId;
            Visited = visited;
            OrganisationId = organisationId;

            UserId = userId;
            Name = name;

            Description = description;
            Rating = rating;

            Center = center;
            Community = community;
            HealthcareProvider = healthcareProvider;
            Experience = experience;
            Feedback = feedback;
        }
    }

    public class Experience
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public Title Title { get; }
        public Description Description { get; }

        public Experience(ExperienceId experienceId, UserId userId, Title title, Description description)
        {
            ExperienceId = experienceId;
            UserId = userId;
            Title = title;
            Description = description;
        }
    }
}