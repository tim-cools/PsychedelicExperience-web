using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PsychedelicExperience.Web.Infrastructure.Security
{
    public static class DataProtectionConfigurationExtensions
    {
        public static IServiceCollection AddAndConfigureDataProtection(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDataProtection(options => options.ApplicationDiscriminator = Assembly.GetEntryAssembly().FullName)
                .Services.PersistKeysInDatabase();

            return services;
        }

        private static IServiceCollection PersistKeysInDatabase(this IServiceCollection services)
        {
            services.Use(DatabaseXmlRepository());

            return services;
        }

        private static ServiceDescriptor DatabaseXmlRepository()
        {
            return ServiceDescriptor.Singleton(services => new DatabaseXmlRepository(services));
        }

        private static void RemoveAllServicesOfType(this IServiceCollection services, Type serviceType)
        {
            for (var index = services.Count - 1; index >= 0; --index)
            {
                var serviceDescriptor = services[index];
                if (serviceDescriptor?.ServiceType == serviceType)
                {
                    services.RemoveAt(index);
                }
            }
        }

        private static void Use(this IServiceCollection services, ServiceDescriptor descriptor)
        {
            services.RemoveAllServicesOfType(descriptor.ServiceType);
            services.Add(descriptor);
        }
    }
}
