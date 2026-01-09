namespace PsychedelicExperience.Psychedelics.Messages.Documents.Queries
{
    public class DocumentsResult
    {
        public DocumentSummary[] Documents { get; set; }
        public long Page { get; set; }
        public long Total { get; set; }
        public long Last { get; set; }
    }
}