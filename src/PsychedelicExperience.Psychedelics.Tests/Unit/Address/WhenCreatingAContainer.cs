using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PsychedelicExperience.Psychedelics.Addresses;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Unit.Address
{
    public class WhenMappingCountries
    {
        [Fact]
        public void ThenColombiaShouldBeMappable()
        {
            var mapper = new CountryMapper();
            mapper.GetCountry("CO").ShouldBe("Colombia");
        }

        [Fact]
        public void ThenCoShouldBeMappable()
        {
            var mapper = new CountryMapper();
            mapper.GetCode("colombia").ShouldBe("CO");
        }

        [Fact]
        public void ThenCoAsCodehouldBeMappable()
        {
            var mapper = new CountryMapper();
            mapper.GetCode("CO").ShouldBe("CO");
        }
    }
}
