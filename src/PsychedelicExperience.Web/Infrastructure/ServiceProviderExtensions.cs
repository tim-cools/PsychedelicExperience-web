using System;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Web.Infrastructure
{
    internal static class ServiceProviderExtensions
    {
        public static IMediator GetMessageDispatcher(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            var service = serviceProvider.GetService(typeof(IMediator)) as IMediator;
            if (service == null)
            {
                throw new InvalidOperationException("MessageDispatcher not registered in container.");
            }

            return service;
        }
    }
}