using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;

namespace PsychedelicExperience.Psychedelics.Tags.Handlers
{
    //todo: refactor, tis is not only about tags anymore...
    public static class TagRepository
    {
        private static readonly IDictionary<TagsDomain, Tag[]> _domains = new Dictionary<TagsDomain, Tag[]> {
            {
                TagsDomain.Experiences, new TagBuilder()
                    .WithCategory("Intent")
                        .WithTags("Personal development", "Spiritual", "Increase creativity", "Sport",
                            "Social", "Psycho Therapy", "Medical use", "Personal research", "Academic research", "Microdosing")
                    .WithCategory("Results")
                        .WithSubCategory("Positive")
                            .WithTags("Mental healing", "Physical healing")
                        .WithSubCategory("Negative")
                            .WithTags("Bad trip", "Health problems")
                    .WithCategory("Environment")
                        .WithTags("Indoor", "Outdoor", "Public area", "Nature", "Festival", "Office", "Health care institution")
                    .WithCategory("Activities")
                        .WithTags("Meditation", "Singing", "Playing music", "Dancing", "Sex")
                    .WithCategory("Company")
                        .WithTags("Solo", "With sober sitter", "With shaman", "Couple", "Small group (<5 people)",
                            "Medium group (> 5 < 10 people)", "Large group (> 10 < 20 people)", "Really large group (> 20 people)",
                            "Public event")
                    //.LoadFromTextFile("Effects")
            },
            {
                TagsDomain.Organisations, new TagBuilder()
                    .WithCategory("Purpose")
                        .WithTags(
                            "Scientific Research", "Drug Policy Reform",
                            "Community", "Education Groups", "Psychotherapy",
                            "Retreat", "Conference", "Religious", "Psychedelic Society",
                            "Harm reduction", "Addiction treatment",
                            "Integration", "Sustainable development",
                            "Indigenous Support", "Tour operator", "Travel agency", "Multimedia",
                            "Distributor", "Shop", "Micodosing")
                    .WithCategory("Medicine")
                        .WithTags(MedicinesTags)
                    .WithCategory("Facilities")
                        .WithTags("Medical Facilities Onsite", "Medical Facilities Nearby", "Therapist Onsite")
                    .WithCategory("Languages")
                        .WithTags("English", "Spanish", "Portuguese", "Dutch", "German", "French")
                    .WithCategory("Status")
                        .WithTags("Defunct")
            },
            {
                //these are mapped in OrganisationViewsController
                TagsDomain.OrganisationTypes, new TagBuilder()
                    .WithCategory("Organisation")
                         .WithTags(
                             "Retreat", "Clinic", "Research", "Community",
                             "Education", "Training Center",
                             "Shop", // , "Consumer", "Medical",
                             // "Consumer Products", "Medical Products",
                             "Business Services",
                             "Media")
                    .WithCategory("Person")
                        .WithTags(
                            "Coach",
                            "Therapist",
                            "Facilitator",
                            "Advocate",
                            "Scientist"
                            //"Advocate",
                            //"Researcher", "Mycologist", "Anthropologist"
                         )
            },
            {
                TagsDomain.Documents, new TagBuilder()
                    .WithCategory("Type")
                        .WithTags("Page", "Infographic", "Blog", "Knowledge")
                    .WithCategory("Topic")
                        .WithTags("History", "Research", "Prohibition", "Summer of love")
                    .WithCategory("Substance")
                        .WithTags(PsychedelicsTags)
            },
            {
                TagsDomain.Events, new TagBuilder()
                    .WithCategory("Medicine")
                    .WithTags(MedicinesTags)
            },
        };

        private static readonly IDictionary<TagsDomain, Tag[]> _domainsEditor = new Dictionary<TagsDomain, Tag[]> {
            {
                TagsDomain.Organisations, new TagBuilder()
                    .WithCategory("Affiliation")
                        .WithTags(
                            "Psychedelic Experience",
                            "Guild Of Guides",
                            "Being True To You",
                            "Veteran Support",
                            "Indigenous Shaman")
            }
        };

        private static string[] PsychedelicsTags =>  MedicinesTags.Union(new[]
        {
            "LSD", "2C-B"
        }).ToArray();

        private static string[] MedicinesTags => new[]
        {
            "Ayahuasca", "Peyote", "San Pedro", "Iboga", "Psilocybin mushroom", "Kambo Frog",
            "Cannabis", "Tabacco", "Rapé", "Sananga", "Brugmansia", "Yopo", "Wilka", "Sanango",
            "Ketamine", "MDMA", "5-MeO-DMT", "DMT"
        };

        internal static Tag[] GetTags(TagsDomain domain, bool editor)
        {
            if (!_domains.TryGetValue(domain, out var tags))
            {
                throw new InvalidOperationException($"Unknown domain: {domain}");
            }

            if (!editor) return tags;

            return _domainsEditor.TryGetValue(domain, out var adminTags)
                ? tags.Concat(adminTags).ToArray()
                : tags;
        }

        public static bool IsInCategory(TagsDomain domain, string category, string tag, bool editor)
        {
            return GetTags(domain, editor)
                .Count(where => where.Category == category && where.Name == tag) > 0;
        }

        public static bool Exists(TagsDomain tagsDomain, string tagName, bool editor)
        {
            var tagNameNormalized = tagName.Generalize();
            return GetTags(tagsDomain, editor)
                .Any(tag => tag.NormalizedName == tagNameNormalized);
        }

        public static Tag[] GetTypes(bool? person = null)
        {
            if (person.HasValue)
            {
                var category = OrganisationCategory(person.Value);
                return GetTags(TagsDomain.Organisations, true)
                    .Where(tag => tag.Category == category)
                    .ToArray();
            }

            return GetTags(TagsDomain.Organisations, true)
                .ToArray();
        }

        public static string OrganisationCategory(bool person) => person ? "Person" : "Organisation";
    }

    public class TagsQueryHandler : QueryHandler<TagsQuery, Tag[]>
    {
        public TagsQueryHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<Tag[]> Execute(TagsQuery query)
        {
            var user = await Session.LoadUserAsync(query.UserId);

            var editor = user != null && user.IsAtLeast(Roles.ContentManager);
            var tags = TagRepository.GetTags(query.Domain, editor);

            var units = query.QueryStringEmpty()
                ? tags
                : FilterTags(tags, query.QueryString);

            return units;
        }

        private Tag[] FilterTags(Tag[] tags, string filter)
        {
            filter = filter.Generalize();
            return tags.Where(criteria => criteria.NormalizedName.Contains(filter)).ToArray();
        }
    }
}