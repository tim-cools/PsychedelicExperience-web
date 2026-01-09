using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;

namespace PsychedelicExperience.Psychedelics.DocumentView.Handlers
{
    public class GetDocumentsHandler : QueryHandler<GetDocuments, DocumentsResult>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetDocumentsHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<DocumentsResult> Execute(GetDocuments getDocumentsQuery)
        {
            var user = await Session.LoadUserAsync(getDocumentsQuery.UserId);      

            var query = new GetDocumentsQueryBuilder(Session, _userInfoResolver)
                .WithPrivacy(user)
                .FilterQueryString(getDocumentsQuery.Query)
                .FilterTags(getDocumentsQuery.Tags)
                .Paging(getDocumentsQuery.Page);

            return await query.Execute();
        }
    }
}
