namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationsTypeTag
    {
        public string Type { get; set; }
        public string Tag { get; set; }
    }

    public class OrganisationsTypeCountry
    {
        public string Type { get; set; }
        public string Country { get; set; }
    }

    public class OrganisationsTypeTagCountry
    {
        public string Type { get; set; }
        public string Tag { get; set; }
        public string Country { get; set; }
    }
}