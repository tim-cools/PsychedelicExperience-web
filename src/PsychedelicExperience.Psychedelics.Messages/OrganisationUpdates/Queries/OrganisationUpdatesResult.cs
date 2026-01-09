using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries
{
    public class OrganisationUpdatesResult
    {
        public OrganisationDetails Organisation { get; set; }
        public OrganisationUpdateSummary[] Updates { get; set; }
        public long Page { get; set; }
        public long Total { get; set; }
        public long Last { get; set; }
    }
}