using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;

namespace  PsychedelicExperience.Psychedelics.Messages.Organisations.Events
{
	public class OrganisationReviewAdded : Event
	{
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public OrganisationId OrganisationId { get; set; }
		public ExperienceId ExperienceId { get; set; }
		public UserId UserId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime Visited { get; set; }
		public ScaleOf5 Rating { get; set; }
		public string Feedback { get; set; }
		public CenterReview Center { get; set; }
		public CommunityReview Community { get; set; }
		public HealthcareProviderReview HealthcareProvider { get; set; }
	}

	public class OrganisationReviewCenterChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public UserId UserId { get; set; }
		public CenterReview Previous { get; set; }
		public CenterReview Review { get; set; }
	}

	public class OrganisationReviewCommunityChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public UserId UserId { get; set; }
		public CommunityReview Previous { get; set; }
		public CommunityReview Review { get; set; }
	}

	public class OrganisationReviewDescriptionChanged : Event
	{
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public UserId UserId { get; set; }
		public string Description { get; set; }
	}

	public class OrganisationReviewHealthcareProviderChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public UserId UserId { get; set; }
		public HealthcareProviderReview Previous { get; set; }
		public HealthcareProviderReview Review { get; set; }
	}

	public class OrganisationReviewNameChanged : Event
	{
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public UserId UserId { get; set; }
		public string Name { get; set; }
	}

	public class OrganisationReviewRated : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public UserId UserId { get; set; }
		public ScaleOf5 PreviousRating { get; set; }
		public ScaleOf5 Rating { get; set; }
	}

	public class OrganisationReviewRemoved : Event
	{
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public ScaleOf5 Rating { get; set; }
	}

	public class OrganisationReviewReported : Event
	{
		public OrganisationReviewId OrganisationReviewId { get; set; }
		public UserId UserId { get; set; }
		public string Reason { get; set; }
		public OrganisationId OrganisationId { get; set; }
	}

}
