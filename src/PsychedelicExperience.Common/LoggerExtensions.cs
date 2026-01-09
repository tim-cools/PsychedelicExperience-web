using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace PsychedelicExperience.Common
{
    public static class LoggerExtensions
    {
        private static readonly EventId _eventId = new EventId(0);

        public static void LogTraceMethod(this ILogger logger, string methodName, object values = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var parameters = PrepareParameters(methodName, values);
            logger.LogTrace(_eventId, Message(values), parameters);
        }

        public static void LogTraceMethod(this ILogger logger, string methodName, Exception exception,
            object values = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var parameters = PrepareParameters(methodName, values);
            logger.LogTrace(_eventId, exception, Message(values), parameters);
        }

        public static void LogDebugMethod(this ILogger logger, string methodName, object values = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var parameters = PrepareParameters(methodName, values);
            logger.LogDebug(_eventId, Message(values), parameters);
        }

        public static void LogInformationMethod(this ILogger logger, string methodName, object values = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var parameters = PrepareParameters(methodName, values);
            logger.LogInformation(_eventId, Message(values), parameters);
        }

        public static void LogWarningMethod(this ILogger logger, string methodName, object values = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var parameters = PrepareParameters(methodName, values);
            logger.LogWarning(_eventId, Message(values), parameters);
        }

        public static void LogWarningMethod(this ILogger logger, string methodName, Exception exception,
            object values = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var parameters = PrepareParameters(methodName, values);
            logger.LogWarning(_eventId, exception, Message(values), parameters);
        }

        public static void LogErrorMethod(this ILogger logger, string methodName, Exception exception,
            object values = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var parameters = PrepareParameters(methodName, values);
            logger.LogError(_eventId, exception, Message(values), parameters);
        }

        public static void LogError(this ILogger logger, Exception exception, object values = null, [CallerMemberName] string methodName = null)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            var parameters = PrepareParameters(methodName, null);
            logger.LogError(_eventId, exception, Message(values), parameters);
        }

        private static string Message(object values)
        {
            return values != null ? "{method}: {values}" : "{method}";
        }

        private static object[] PrepareParameters(string methodName, object values)
        {
            var parameters = new object[values != null ? 2 : 1];
            parameters[0] = methodName;
            if (values != null)
            {
                parameters[1] = values;
            }
            return parameters;
        }
    }
}