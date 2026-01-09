
namespace PsychedelicExperience.Common.Messages
{
    public interface IHandleCommand<in TMessage> : IAsyncRequestHandler<TMessage, Result>
        where TMessage : IRequest<Result>
    {
    }
}