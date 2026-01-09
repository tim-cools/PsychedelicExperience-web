
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;

namespace PsychedelicExperience.Psychedelics.DocumentView.Handlers
{
    public class GetDocumentsSitemapHandler : QueryHandler<GetDocumentsSitemap, DocumentsSitemapResult>
    {
        public GetDocumentsSitemapHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<DocumentsSitemapResult> Execute(GetDocumentsSitemap getDocumentsQuery)
        {
            var documents = await Session.Query<Document>()
                .Where(document => !document.Removed && document.Status == DocumentStatus.Published)
                .Select(document => new { document.Id, document.Name, document.Tags, document.Slug })
                .ToListAsync();
            
            var entries = documents.Select(document => new DocumentsSitemapEntry()
                {
                    Id = document.Id,
                    Name = document.Name,
                    Slug = document.Slug,
                    Tags = document.Tags
                })
                .ToArray();

            var tags = NormalizeExisting(entries, organisation => organisation.Tags);

            return new DocumentsSitemapResult
            {
                Documents = entries,
                Tags = tags
            };
        }

        private static string[] NormalizeExisting(DocumentsSitemapEntry[] documents, Func<DocumentsSitemapEntry, IEnumerable<string>> collectionSelector)
        {
            return documents
                .SelectMany(collectionSelector)
                .Distinct()
                .Where(value => value != null)
                .Select(value => value.NormalizeForUrl())
                .ToArray();
        }
    }
}
