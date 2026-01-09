using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using Marten.Linq;
using Marten.Services.Includes;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.DocumentView.Handlers
{
    public class GetDocumentsQueryBuilder
    {
        public const int PageSize = 20;

        private ExpressionStarter<Document> _where;
        private readonly IQuerySession _session;
        private readonly IUserInfoResolver _userInfoResolver;
        private int _page;
        private User _user;

        public GetDocumentsQueryBuilder(IQuerySession session, IUserInfoResolver userInfoResolver)
        {
            _session = session;
            _userInfoResolver = userInfoResolver;
            _where = PredicateBuilder.New<Document>();
        }

        public GetDocumentsQueryBuilder WithPrivacy(User user)
        {
            _user = user;
            _where = user?.IsAtLeast(Roles.ContentManager) == true 
                ? _where.And(document => !document.Removed) 
                : _where.And(document => !document.Removed && document.Status == DocumentStatus.Published);

            return this;
        }

        public GetDocumentsQueryBuilder FilterQueryString(string queryString)
        {
            queryString = queryString.NormalizeForSearch();

            if (string.IsNullOrWhiteSpace(queryString)) return this;

            var query = PredicateBuilder.New<Document>()
                .Or(document => document.SearchString.Contains(queryString))
                .Or(document => document.TagsNormalized.Contains(queryString));

            _where = _where.And(query);

            return this;
        }

        public GetDocumentsQueryBuilder FilterTags(string[] tags)
        {
            if (tags == null || tags.Length == 0) return this;

            foreach (var tag in tags)
            {
                var value = tag.NormalizeForSearch();
                _where = _where.And(document => document.TagsNormalized.Contains(value));
            }
            return this;
        }

        public GetDocumentsQueryBuilder Paging(int page)
        {
            _page = page;
            return this;
        }

        private DocumentSummary[] Map(IReadOnlyList<Document> documents, List<TopicInteraction> interactions)
        {
            return documents
                .Select(value => new
                {
                    Document = value,
                    Interaction = interactions.FirstOrDefault(interaction => interaction?.Id == value.Id)
                })
                .Select(value => value.Document.MapSummary(_user, value.Interaction, _userInfoResolver))
                .ToArray();
        }

        public async Task<DocumentsResult> Execute()
        {
            var interactions = new List<TopicInteraction>();

            var skip = _page*PageSize;
            var query = _session.Query<Document>()
                .Stats(out var stats)
                .Include(document => document.Id, interactions, JoinType.LeftOuter)
                .Where(_where)
                .OrderBy(document => document.Name)
                .Skip(skip)
                .Take(PageSize);

            var documents = await query.ToListAsync();

            return new DocumentsResult
            {
                Documents = Map(documents, interactions),
                Page = _page,
                Total = stats.TotalResults,
                Last = skip + documents.Count
            };
        }
    }
}