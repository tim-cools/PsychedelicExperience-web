using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Substances.Queries;

namespace PsychedelicExperience.Psychedelics.SubstanceView.Handlers
{
    public static class SubstanscesRepository
    {
        public static readonly Substance[] Substances =
        {
            new Substance(new Guid("74a4a446-d575-405e-9471-a3d21b7e9aa8"), "Psilocybe (Magic Mushrooms)",
                new[] { "Fresh Mushrooms", "Dry Mushrooms", "Truffels", "HCI Extract" },
                new[] { "gr", "mg" },
                new[] { "Oral" }),

            new Substance(new Guid("4a1bcc94-0adc-4450-8ff6-c80e109dfa20"), "LSD",
                new[] { "blotters", "microdot", "liquid" },
                new[] { "μg", "piece", "drops" },
                new[] { "Oral" }),

            new Substance(new Guid("3841c791-12a7-4850-9571-7fca6c13db0d"), "Mescaline (Cacti)",
                new[] { "Fresh Cactus", "Dried Cactus", "Synthetic" },
                new[] { "mg", "gr", "blotter", "drop" },
                new[] { "Oral" }),

            new Substance(new Guid("e085fd2e-6c20-4c99-b0cb-456b81715c07"), "Ayahuasca",
                new[] { "Tea" },
                new[] { "cups" },
                new[] { "Oral" }),

            new Substance(new Guid("373fc52c-1704-407c-848e-ee15f513478d"), "Iboga",
                new[] { "Ibogaine hydrochloride", "Total alkaloid" },
                new[] { "gr" },
                new[] { "Oral" }),

            new Substance(new Guid("078a7c35-b8d9-4cbc-aa56-6c02d11f0cc2"), "DMT",
                new[] { "Synthetic Powder", "Salt Extract", "Freebase Extract" },
                new[] { "mg" },
                new[] { "Smoked", "Oral", "Intravenous" }),

            new Substance(new Guid("078a7c35-b8d9-4cbc-aa56-6c02d11f0cc3"), "5-MeO-DMT",
                new[] { "Synthetic Powder", "Salt", "Freebase" },
                new[] { "mg" },
                new[] { "Smoked", "Oral", "Intravenous", "Rectally" }),
            
            new Substance(new Guid("078a7c35-b8d9-4cbc-aa56-6c02d11f0cc4"), "Bufo alvarius",
                new[] { "Dried Venom" },
                new[] { "mg" },
                new[] { "Smoked" }),

            new Substance(new Guid("1c9828a5-bd07-4fa3-bf9b-e71167d6f3d0"), "MDMA",
                new[] { "Pills", "Powder", "Crystals" },
                new[] { "mg", "piece" },
                new[] { "Oral" }),

            new Substance(new Guid("1c9828a5-bd07-4fa3-bf9b-e71167d6f3d1"), "2C-B",
                new[] { "Powder", "Pill" },
                new[] { "mg", "piece" },
                new[] { "Oral", "Snorted" }),

            new Substance(new Guid("1c9828a5-bd07-4fa3-bf9b-e71167d6f3d2"), "2C-C",
                new[] { "Powder", "Pills" },
                new[] { "mg", "piece" },
                new[] { "Oral", "Snorted" }),

            new Substance(new Guid("1c9828a5-bd07-4fa3-bf9b-e71167d6f3d3"), "2C-D",
                new[] { "Powder", "Pills" },
                new[] { "mg", "piece" },
                new[] { "Oral", "Snorted" }),

            new Substance(new Guid("1c9828a5-bd07-4fa3-bf9b-e71167d6f3d4"), "2C-E",
                new[] { "Powder", "Pills" },
                new[] { "mg", "piece" },
                new[] { "Oral", "Snorted" }),

            new Substance(new Guid("1c9828a5-bd07-4fa3-bf9b-e71167d6f3d5"), "2C-H",
                new[] { "Powder", "Pills" },
                new[] { "mg", "piece" },
                new[] { "Oral", "Snorted" }),

            new Substance(new Guid("1c9828a5-bd07-4fa3-bf9b-e71167d6f3d6"), "2C-I",
                new[] { "Powder", "Pills" },
                new[] { "mg", "piece" },
                new[] { "Oral", "Snorted" }),

            new Substance(new Guid("1c9828a5-bd07-4fa3-bf9b-e71167d6f3d7"), "Cannabis",
                new[] { "Dried Buds", "Hashies", "Dabs" },
                new[] { "mg", "gr" },
                new[] { "Smoked", "Oral" }),


            //new Substance(new Guid("526b2d02-7e02-475f-ad54-bab287ce71ad"),
            //new Substance(new Guid("f8c3a477-e55b-4ad7-be17-489dd8499f3b"),
            //new Substance(new Guid("ae1b48d3-69a1-4118-90f0-1fc271908fde")
        };
    }

    public class SubstancesQueryHandler : QueryHandler<SubstancesQuery, SubstanceResult>
    {

        public SubstancesQueryHandler(IQuerySession session) : base(session)
        {
        }

        protected override Task<SubstanceResult> Execute(SubstancesQuery query)
        {
            var substances = query.QueryStringEmpty()
                ? DefaultSubstances()
                : FilterSubstances(query.QueryString);

            return Task.FromResult(new SubstanceResult
            {
                Substances = substances
            });
        }

        private Substance[] DefaultSubstances()
        {
            return SubstanscesRepository.Substances;
        }

        private Substance[] FilterSubstances(string filter)
        {
            filter = filter.ToLowerInvariant();
            return SubstanscesRepository.Substances.Where(criteria => criteria.NormalizedName.Contains(filter)).ToArray();
        }
    }
}
