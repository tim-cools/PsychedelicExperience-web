using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace PsychedelicExperience.Common.Tests.Unit.ContainerSpecifications
{
    public class DummyConfiguration : IConfigurationRoot
    {
        public IConfigurationSection GetSection(string key)
        {
            return null;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            yield break;
        }

        public IChangeToken GetReloadToken()
        {
            return null;
        }

        public string this[string key]
        {
            get => null;
            set { }
        }

        public void Reload()
        {
        }

        public IEnumerable<IConfigurationProvider> Providers { get; }
    }
}