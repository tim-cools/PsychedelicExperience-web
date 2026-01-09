using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Queries
{
    public class GetDocuments : IRequest<DocumentsResult>
    {
        public UserId UserId { get; }
        public string Query { get; }
        public int Page { get; }
        public string[] Tags { get; set; }

        public GetDocuments(UserId userId, string[] tags = null, string query = null, int page = 0)
        {
            UserId = userId;
            Tags = tags;
            Query = query;
            Page = page;
        }
    }
}