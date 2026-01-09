using Marten.Events;

namespace PsychedelicExperience.Common
{
    public interface IInitializeEvents
    {
        void Initialize(EventGraph events);
    }
}