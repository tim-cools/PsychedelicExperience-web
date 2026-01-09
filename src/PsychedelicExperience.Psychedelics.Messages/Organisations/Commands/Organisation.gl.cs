using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace  PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
	public class AddOrganisation : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public bool Person { get; set; }
		public string[] Types { get; set; }
		public Name Name { get; set; }
		public string Description { get; set; }
		public Contact[] Contacts { get; set; }
		public string[] Tags { get; set; }
		public Address Address { get; set; }
		public Center Center { get; set; }
		public Community Community { get; set; }
		public HealthcareProvider HealthcareProvider { get; set; }
		public Practitioner Practitioner { get; set; }

		public AddOrganisation(OrganisationId organisationId, UserId userId, bool person, string[] types, Name name, string description, Contact[] contacts, string[] tags, Address address, Center center, Community community, HealthcareProvider healthcareProvider, Practitioner practitioner) : base()
		{
			OrganisationId = organisationId;
			UserId = userId;
			Person = person;
			Types = types;
			Name = name;
			Description = description;
			Contacts = contacts;
			Tags = tags;
			Address = address;
			Center = center;
			Community = community;
			HealthcareProvider = healthcareProvider;
			Practitioner = practitioner;
		}
	}

	public class AddOrganisationContact : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }

		public AddOrganisationContact(OrganisationId organisationId, UserId userId, string type, string value) : base()
		{
			OrganisationId = organisationId;
			UserId = userId;
			Type = type;
			Value = value;
		}
	}

	public class ChangeOrganisationAddress : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public Address Address { get; set; }

		public ChangeOrganisationAddress(OrganisationId organisationId, UserId userId, Address address) : base()
		{
			OrganisationId = organisationId;
			UserId = userId;
			Address = address;
		}
	}

	public class AddOrganisationOwner : IRequest<AddOrganisationOwnerResult>
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public EMail UserEmail { get; set; }
		public UserId OwnerId { get; set; }

		public AddOrganisationOwner(OrganisationId organisationId, UserId userId, EMail userEmail, UserId ownerId) : base()
		{
			OrganisationId = organisationId;
			UserId = userId;
			UserEmail = userEmail;
			OwnerId = ownerId;
		}
	}

	public class AddOrganisationOwnerResult : Result
	{
		public Guid OwnerId { get; set; }

		public AddOrganisationOwnerResult() : base()
		{
		}

		public AddOrganisationOwnerResult(bool success, params ValidationError[] errors) : base(success, errors)
		{
		}

		public AddOrganisationOwnerResult(Guid ownerId) : base(true)
		{
			OwnerId = ownerId;
		}
	}

	public class AddOrganisationTag : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string TagName { get; set; }

		public AddOrganisationTag(OrganisationId organisationId, UserId userId, string tagName) : base()
		{
			OrganisationId = organisationId;
			UserId = userId;
			TagName = tagName;
		}
	}

	public class RemoveOrganisationTag : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string TagName { get; set; }

		public RemoveOrganisationTag(OrganisationId organisationId, UserId userId, string tagName) : base()
		{
			OrganisationId = organisationId;
			UserId = userId;
			TagName = tagName;
		}
	}

	public class AddOrganisationType : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Type { get; set; }

		public AddOrganisationType(OrganisationId organisationId, UserId userId, string type) : base()
		{
			OrganisationId = organisationId;
			UserId = userId;
			Type = type;
		}
	}

	public class RemoveOrganisationType : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public UserId UserId { get; set; }
		public string Type { get; set; }

		public RemoveOrganisationType(OrganisationId organisationId, UserId userId, string type) : base()
		{
			OrganisationId = organisationId;
			UserId = userId;
			Type = type;
		}
	}

	public class LinkOrganisation : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public OrganisationId TargetId { get; set; }
		public UserId UserId { get; set; }
		public string Relation { get; set; }

		public LinkOrganisation(OrganisationId organisationId, OrganisationId targetId, UserId userId, string relation) : base()
		{
			OrganisationId = organisationId;
			TargetId = targetId;
			UserId = userId;
			Relation = relation;
		}
	}

	public class UnlinkOrganisation : IRequest<Result>
	{
		public OrganisationId OrganisationId { get; set; }
		public OrganisationId TargetId { get; set; }
		public UserId UserId { get; set; }
		public string Relation { get; set; }

		public UnlinkOrganisation(OrganisationId organisationId, OrganisationId targetId, UserId userId, string relation) : base()
		{
			OrganisationId = organisationId;
			TargetId = targetId;
			UserId = userId;
			Relation = relation;
		}
	}

}
