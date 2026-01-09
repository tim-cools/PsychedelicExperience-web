using System;
using System.Linq;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
   public class Practitioner
    {
        public General General { get; set; }
        public Work Work { get; set; }

        public Therapist Therapist { get; set; }
        public Coach Coach { get; set; }
        public Facilitator Facilitator { get; set; }

        public string NormalizeForSearch() =>
            NormalizeForSearch(
                General?.Story,
                Work?.Approach
                );

        private string NormalizeForSearch(params string[] normalizeForSearch)
        {
            return string.Join(' ', normalizeForSearch.Where(value => value != null).Select(value => value.NormalizeForSearch()));
        }
    }


    public class General
    {
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string Pronouns { get; set; }
        public DateTime BirthDate { get; set; }
        public string SexualOrientation { get; set; }
        public string Ethnicity { get; set; }
        public string Background { get; set; }
        public string PsychedelicExperience { get; set; }
        public string Story { get; set; }
    }

    public class Work
    {
        public DateTime? StartedSince { get; set; }
        public string Medicines { get; set; }
        public string SpiritualBackground { get; set; }
        public string Approach { get; set; }
        public string Clients { get; set; }
        public string[] Practices { get; set; }
        public string Training { get; set; }
        public string[] Affiliations { get; set; }
        public WorkPrice WorkPrice { get; set; }
        public string Deals { get; set; }
    }

    public enum WorkPrice
    {
        Inexpensive,  //$   < 80
        Standard,     //$$  > 80$
        Expensive,    //$$$  > 120$
        VeryExpensive //$$$$ > 200$
    }

    public class Therapist
    {
        public string[] Specialities { get; set; }
        public string[] Accreditations { get; set; }
    }

    public class Coach
    {
        public string[] Specialities { get; set; }
        public string[] Certifications { get; set; }
        public string[] ProfessionalBody { get; set; }
        public CoachExperience Experience { get; set; }
    }

    public class CoachExperience
    {
        public int? Hours { get; set; }
        public int? Clients { get; set; }
    }

    public class Facilitator
    {
        public int? Sessions { get; set; }
        public int? GroupSizeMinimum { get; set; }
        public int? GroupSizeMaximum { get; set; }
        public string[] Roles { get; set; }
        public string Lineage { get; set; }
    }
}