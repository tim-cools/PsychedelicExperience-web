using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;

namespace  PsychedelicExperience.Psychedelics.Messages.Organisations.Events
{
	public enum ScaleOf5
	{
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4,
		Five = 5,
	}

	public class OrganisationAdded : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public bool Person { get; set; }
		public string[] Types { get; set; }
		public Name Name { get; set; }
		public string Description { get; set; }
		public string Website { get; set; }
		public EMail EMail { get; set; }
		public string Phone { get; set; }
		public Address Address { get; set; }
		public string[] Tags { get; set; }
		public Contact[] Contacts { get; set; }
		public Center Center { get; set; }
		public Community Community { get; set; }
		public HealthcareProvider HealthcareProvider { get; set; }
		public Practitioner Practitioner { get; set; }
	}

	public class OrganisationAddressChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Address Address { get; set; }
	}


	public class OrganisationPersonChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public bool Person { get; set; }
	}

	public class OrganisationCenterChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Center Center { get; set; }
	}

	public class OrganisationPractitionerChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Practitioner Practitioner { get; set; }
	}

	public class OrganisationCommunityChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Community Community { get; set; }
	}

	public class OrganisationContactAddded : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationContacted : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string EMail { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
		public string UserName { get; set; }
		public string OrganisationName { get; set; }
		public string OrganisationEmail { get; set; }
	}

	public class OrganisationContactRemoved : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationDescriptionChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Description { get; set; }
	}

	public class OrganisationHealthcareProviderChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public HealthcareProvider HealthcareProvider { get; set; }
	}

	public class OrganisationInfoRemoved : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationInfoSet : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationNameChanged : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Name Name { get; set; }
	}

	public class OrganisationOwnerAdded : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public UserId OwnerId { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationOwnerConfirmed : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
	}

	public class OrganisationOwnerInvited : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public UserId OwnerId { get; set; }
		public EMail UserEmail { get; set; }
	}

	public class OrganisationOwnerRemoved : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public UserId OwnerId { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationPhotoRemoved : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public PhotoId PhotoId { get; set; }
		public UserId UserId { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationPhotosAdded : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Photo[] Photos { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationRemoved : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationReported : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }

		public Name OrganisationName { get; set; }
		public string Reason { get; set; }
	}

	public class OrganisationTagAdded : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string TagName { get; set; }
	}

	public class OrganisationTagRemoved : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string TagName { get; set; }
	}

	public class OrganisationTypeAdded : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Type { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationTypeRemoved : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Type { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationWarningRemoved : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationWarningSet : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationLinked : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public OrganisationId TargetId { get; set; }
		public UserId UserId { get; set; }
		public string Relation { get; set; }
		public Name OrganisationName { get; set; }
	}

	public class OrganisationUnlinked : Event
	{
		public OrganisationId OrganisationId { get; set; }
		public OrganisationId TargetId { get; set; }
		public UserId UserId { get; set; }
		public string Relation { get; set; }
		public Name OrganisationName { get; set; }
	}

}
