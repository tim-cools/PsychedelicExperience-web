
using System.Threading.Tasks;

namespace PsychedelicExperience.Common.Messages
{
    public interface IAsyncRequestHandler<in TMessage, TResult> where TMessage : IRequest<TResult>
    {
        Task<TResult> Handle(TMessage query);
    }

    public interface IRequest<out TResult>
    {
    }
}