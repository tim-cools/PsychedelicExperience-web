using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.Users;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdateView
{
    public class OrganisationUpdate
    {
        public Guid Id { get; set; }
        public Guid OrganisationId { get; set; }

        public OrganisationUpdatePrivacy Privacy { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }

        public DateTime Created { get; set; }
        public UserId CreatedBy { get; set; }

        public DateTime LastUpdated { get; set; }
        public UserId LastUpdatedBy { get; set; }

        public bool Removed { get; set; }

        public string GetUrl()
        {
            return $"/organisation/{new ShortGuid(OrganisationId)}/updates/{(ShortGuid)Id}";
        }

        public bool CanView(bool isMember, bool isAdministrator)
        {
            return Privacy == OrganisationUpdatePrivacy.Public || isMember || isAdministrator;
        }
    }

    public enum OrganisationUpdatePrivacy
    {
        MembersOnly,
        Public
    }
}