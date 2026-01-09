namespace PsychedelicExperience.Common.Messages
{
    public interface IHandleEvent<T>
    {
        void Handle(T @event);
    }
}