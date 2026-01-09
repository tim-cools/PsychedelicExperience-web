using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Substances.Queries
{
    public class Substance
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public IList<Form> DefaultForms { get; set; }
        public IList<Unit> DefaultUnits { get; set; }
        public IList<Method> DefaultMethods { get; set; }

        public Substance()
        {
        }
        public Substance(Guid guid, string name, string[] defaultForms, string[] defaultUnits, string[] defaultMethods)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Id = guid;
            Name = name;
            NormalizedName = name.NormalizeForSearch();

            DefaultForms = defaultForms.Select(form => new Form(form)).ToArray();
            DefaultUnits = defaultUnits.Select(unit => new Unit(unit)).ToArray();
            DefaultMethods = defaultMethods.Select(method => new Method(method)).ToArray();
        }
    }
}