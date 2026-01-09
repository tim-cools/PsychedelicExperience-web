using System;
using System.Threading.Tasks;
using StructureMap;

namespace PsychedelicExperience.Common.Messages
{
    public class Mediator : IMediator
    {
        private readonly IContainer _container;

        public Mediator(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async Task<TResult> Send<TResult>(IRequest<TResult> message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            using (var context = _container.CreateChildContainer())
            {
                var handler = GetHandler(context, message);

                return await handler.Handle((dynamic)message);
            }
        }

        private static dynamic GetHandler<TResult>(IContainer container, IRequest<TResult> message)
        {
            var handlerType = typeof(IAsyncRequestHandler<,>)
                .MakeGenericType(message.GetType(), typeof(TResult));

            try
            {
                return container.GetInstance(handlerType);
            }
            catch (StructureMapConfigurationException exception)
            {
                throw new InvalidOperationException(
                    $"Handler {handlerType} for message : {message.GetType()} not found. " +
                    $"Did you call corresponding ContainerInitializer method while configuring the container?", exception);
            }
        }
    }
}