using System.Threading.Tasks;

namespace PsychedelicExperience.Common.Messages
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request);
    }
}