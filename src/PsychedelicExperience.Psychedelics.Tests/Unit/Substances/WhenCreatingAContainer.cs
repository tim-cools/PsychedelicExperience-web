using System;
using System.IO;
using System.Linq;
using Baseline;
using Newtonsoft.Json;
using PsychedelicExperience.Psychedelics.Messages.Substances.Queries;
using PsychedelicExperience.Psychedelics.SubstanceView.Handlers;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Unit.Substances
{
    public class WhenSerialising
    {
        [Fact]
        public void Substances()
        {
            var serialiser = new JsonSerializer();

            PsychedelicsRegistry.CustomTypeConverters
                .Select(type => Activator.CreateInstance(type) as JsonConverter)
                .Each(serialiser.Converters.Add);

            var writer = new StringWriter();
            serialiser.Serialize(writer, SubstanscesRepository.Substances);
        }
    }
}
