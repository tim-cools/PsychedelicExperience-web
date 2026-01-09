using System;
using System.Diagnostics;
using PsychedelicExperience.Common.Messages;
using Shouldly;
using Xunit.Abstractions;

namespace PsychedelicExperience.Common.Tests.Messages
{
    public static class MessageDispatcherExtensions
    {
        public static T ExecuteNowWithTimeout<T>(this IMediator messageDispatcher, IRequest<T> message, ITestOutputHelper testOutputHelper = null)
        {
            if (messageDispatcher == null) throw new ArgumentNullException(nameof(messageDispatcher));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var stopWatch = new Stopwatch();
            var task = messageDispatcher.Send(message);
            if (!task.Wait(60000))
            {
                throw new InvalidOperationException($"Timeout on executing  '{typeof(T)}' after {stopWatch.ElapsedMilliseconds} ms");
            }

            testOutputHelper?.WriteLine($"Executed '{typeof(T)}' in {stopWatch.ElapsedMilliseconds} ms");

            return task.Result;
        }

        public static TestContext<IMediator> ShouldSucceed<T>(this TestContext<IMediator> context, IRequest<T> message, ITestOutputHelper testOutputHelper = null) where T : Result
        {
            var result = context.Service.ExecuteNowWithTimeout(message);
            result.Succeeded.ShouldBeTrue(result.ToString);
            return context;
        }
    }
}
