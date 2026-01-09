using System;
using StructureMap;

namespace PsychedelicExperience.Common.Tests
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly IContainer _container;

        public ServiceProvider(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            _container = container;
        }

        public Object GetService(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }
}