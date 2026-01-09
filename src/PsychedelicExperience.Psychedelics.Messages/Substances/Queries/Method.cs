using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Substances.Queries
{
    public class Method
    {
        public string Name { get; }
        public string NormalizedName { get; }

        public Method(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
            NormalizedName = name.NormalizeForSearch();
        }
    }
}