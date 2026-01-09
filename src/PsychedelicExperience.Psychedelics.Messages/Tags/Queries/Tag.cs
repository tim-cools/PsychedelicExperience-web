using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Tags.Queries
{
    public class Tag
    {
        public string Category { get; }
        public string SubCategory { get; set; }
        public string Name { get; }
        public string NormalizedName { get; }

        public Tag(string category, string subCategory, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Category = category;
            SubCategory = subCategory;
            Name = name;
            NormalizedName = name.Generalize();
        }
    }
}