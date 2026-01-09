using System;
using System.Collections.Generic;
using Baseline;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;

namespace PsychedelicExperience.Web.ViewModels.Api
{
    public class PostOrganisationRequest
    {
        public Name Name { get; set; }

        public string Description { get; set; }
        public Contact[] Contacts { get; set; }
        
        public bool Person { get; set; }
        public string[] Types { get; set; }

        /// <summary>
        /// This property is obsolete and should be removed
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// This property is obsolete and should be removed
        /// </summary>
        public EMail Email { get; set; }
        /// <summary>
        /// This property is obsolete and should be removed
        /// </summary>
        public string Phone { get; set; }

        public string[] Tags { get; set; }
        public GooglePlace Address { get; set; }

        public Center Center { get; set; }
        public Community Community { get; set; }
        public HealthcareProvider HealthcareProvider { get; set; }
        public Practitioner Practitioner { get; set; }

        public AddOrganisation ToCommand(OrganisationId id, UserId userId)
        {
            return new AddOrganisation(
                id,
                userId,
                Person,
                Types,
                Name,
                Description,
                MapContacts(),
                Tags,
                Address.ToAddress(),
                Center,
                Community,
                HealthcareProvider,
                Practitioner);
        }

        private Psychedelics.Messages.Organisations.Contact[] MapContacts()
        {
            var result = new List<Psychedelics.Messages.Organisations.Contact>();
            Contacts?.Each(contact => result.Add(new Psychedelics.Messages.Organisations.Contact { Type = contact.Type, Value = contact.Value }));
            if (!string.IsNullOrWhiteSpace(Website))
            {
                result.Add(new Psychedelics.Messages.Organisations.Contact { Type = ContactTypes.WebSite, Value = Website });
            }
            if (!string.IsNullOrWhiteSpace(Email?.Value))
            {
                result.Add(new Psychedelics.Messages.Organisations.Contact { Type = ContactTypes.EMail, Value = Email.Value });
            }
            if (!string.IsNullOrWhiteSpace(Phone))
            {
                result.Add(new Psychedelics.Messages.Organisations.Contact { Type = ContactTypes.Phone, Value = Phone });
            }
            return result.ToArray();
        }

    }

    public class PostReviewRequest
    {
        public string Name { get; set; }
        public DateTime Visited { get; set; }
        public Rating Rating { get; set; }
        public string Feedback { get; set; }

        public CenterReview Center { get; set; }
        public CommunityReview Community { get; set; }
        public HealthcareProviderReview HealthcareProvider { get; set; }

        public PostExperience Experience { get; set; }

        public AddOrganisationReview ToCommand(OrganisationId id, OrganisationReviewId organisationReviewId, UserId userId)
        {
            var experience = Experience != null ? new Experience(ExperienceId.New(), userId, Experience.Title, Experience.Description) : null;

            return new AddOrganisationReview(organisationReviewId,
                Visited,
                id,
                userId,
                Name,
                Rating.Description,
                Rating.Value,
                Center,
                Community,
                HealthcareProvider,
                experience,
                Feedback);
        }
    }

    public class PostExperience
    {
        public Title Title { get; set; }
        public Description Description { get; set; }
    }
}