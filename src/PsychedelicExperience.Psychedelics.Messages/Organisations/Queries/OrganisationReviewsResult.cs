namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationReviewsResult
    {
        public OrganisationsReviewSummary[] Organisations { get; set; }
        public long Page { get; set; }
        public long Total { get; set; }
        public long Last { get; set; }
    }
}