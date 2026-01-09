using System;
using Microsoft.Extensions.Logging;

namespace PsychedelicExperience.Common.Tests
{
    public class DummyLogger : ILogger, IDisposable
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var formatted = formatter(state, exception);
            Console.WriteLine($"{logLevel}:{eventId} {formatted}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }
    }

    public class DummyLogger<T> : DummyLogger, ILogger<T>
    {
    }
}