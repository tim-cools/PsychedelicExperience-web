
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;

namespace PsychedelicExperience.Psychedelics.ExperienceView.Handlers
{
    public class GetExperiencesSitemapHandler : QueryHandler<GetExperiencesSitemap, ExperiencesSitemapResult>
    {
        public GetExperiencesSitemapHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<ExperiencesSitemapResult> Execute(GetExperiencesSitemap getExperiencesQuery)
        {
            var experiences = await Session.Query<Experience>()
                .Where(experience => experience.PrivacyLevel != PrivacyLevel.Private)
                .Select(experience => new { experience.Id, experience.Title, experience.Tags, experience.Doses })
                .ToListAsync();
            
            var entries = experiences.Select(experience => new ExperiencesSitemapEntry()
                {
                    Id = experience.Id,
                    Title = experience.Title,
                    Substances = experience.Doses.Select(dose => dose.Substance).ToList(),
                    Tags = experience.Tags
                })
                .ToArray();

            var tags = NormalizeExisting(entries, organisation => organisation.Tags);
            var substances = NormalizeExisting(entries, organisation => organisation.Substances);

            //todo: we currently don't have enough data to show all pages
            //Change to all tags when we have enough content on the web-site
            //var tags = TagRepository.GetTags(TagsDomain.Experiences)
            //        .Select(category => category.Name.NormalizeForUrl())
            //        .ToArray();

            //var substances = SubstanscesRepository.Substances
            //    .Select(substance => substance.Name.NormalizeForUrl())
            //    .ToArray();

            return new ExperiencesSitemapResult
            {
                Experiences = entries,
                Tags = tags,
                Substances = substances
            };
        }

        private static string[] NormalizeExisting(ExperiencesSitemapEntry[] experiences, Func<ExperiencesSitemapEntry, IEnumerable<string>> collectionSelector)
        {
            return experiences
                .SelectMany(collectionSelector)
                .Distinct()
                .Where(value => value != null)
                .Select(value => value.NormalizeForUrl())
                .ToArray();
        }
    }
}
