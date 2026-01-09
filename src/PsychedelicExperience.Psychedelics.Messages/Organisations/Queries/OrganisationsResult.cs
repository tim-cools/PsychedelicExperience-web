namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationsResult
    {
    }

    public class JsonOrganisationsResult : OrganisationsResult
    {
        public OrganisationSummary[] Organisations { get; set; }
        public long Page { get; set; }
        public long Total { get; set; }
        public long Last { get; set; }
    }

    public class CsvOrganisationsResult : OrganisationsResult
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
    }


}