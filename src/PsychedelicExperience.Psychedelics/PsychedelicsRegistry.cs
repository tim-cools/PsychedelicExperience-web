
using System;
using System.Collections.Generic;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.StructureMap;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.ReportMail;
using StructureMap;

namespace PsychedelicExperience.Psychedelics
{
    public class PsychedelicsRegistry : Registry
    {
        public static IList<Type> CustomTypeConverters { get; } = JsonConverterResolver.Resolve<PsychedelicsRegistry, ExperienceId>();

        public PsychedelicsRegistry()
        {
            Scan(options => { 
                options.TheCallingAssembly();
                options.Include(type => type.Name.EndsWith("Projection"));
                options.Include(type => type.Name.EndsWith("Validator"));
                options.Include(type => type.Name.EndsWith("Provider"));
                options.Include(type => type.Name.EndsWith("Handler"));
                options.Include(type => type.Name.EndsWith("Mapper"));
                options.Include(type => type.Name.EndsWith("Notification"));
                options.Include(type => type.Name.EndsWith("Job"));

                options.Convention<AllInterfacesConvention>();
            });

            For<MartenRegistry>().Use<PsychedelicsDocumentRegistry>();

            foreach (var customTypeConverter in CustomTypeConverters)
            {
                For(typeof(CustomTypeConverter)).Use(customTypeConverter);
            }
        }
    }
}