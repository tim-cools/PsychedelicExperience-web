using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries
{
    public class OrganisationUpdateResult
    {
        public OrganisationDetails Organisation { get; set; }
        public OrganisationUpdateDetails Update { get; set; }
    }
}