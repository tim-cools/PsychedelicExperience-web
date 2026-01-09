using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Substances.Queries
{
    public class Unit
    {
        public string Name { get; }
        public string NormalizedName { get; }

        public Unit(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
            NormalizedName = name.NormalizeForSearch();
        }
    }
}