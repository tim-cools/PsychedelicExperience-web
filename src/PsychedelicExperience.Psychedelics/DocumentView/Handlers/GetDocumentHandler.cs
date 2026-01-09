using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;

namespace PsychedelicExperience.Psychedelics.DocumentView.Handlers
{
    public class GetDocumentHandler : QueryHandler<GetDocument, DocumentDetails>
    {
        private readonly IUserInfoResolver _userInfoResolver;
        private readonly IUserDataProtector _userDataProtector;

        public GetDocumentHandler(IQuerySession session, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector) : base(session)
        {
            _userInfoResolver = userInfoResolver;
            _userDataProtector = userDataProtector;
        }

        protected override async Task<DocumentDetails> Execute(GetDocument query)
        {
            var document = await GetDocument(query);
            var user = await Session.LoadUserAsync(query.UserId);

            return document?.MapDetails(user, _userInfoResolver, _userDataProtector);
        }

        private async Task<Document> GetDocument(GetDocument query)
        {
            return query.Slug != null
                ? await DocumentBySlug(query)
                : await DocumentById(query);
        }

        private async Task<Document> DocumentById(GetDocument query)
        {
            return await Session.LoadAsync<Document>(query.DocumentId);
        }

        private async Task<Document> DocumentBySlug(GetDocument query)
        {
            var querySlug = query.Slug.TrimStart('/');
            return await Session.Query<Document>()
                .Where(document => document.Slug == querySlug)
                .FirstOrDefaultAsync();
        }
    }
}
