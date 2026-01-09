using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events
{
    public class OrganisationUpdateSubjectChanged : Event
    {
        public UserId UserId { get; set; }
        public OrganisationId OrganisationId { get; set; }
        public OrganisationUpdateId UpdateId { get; set; }
        public string Subject { get; set; }
    }
}