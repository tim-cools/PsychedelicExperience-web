using System;
using System.Threading;
using System.Threading.Tasks;

namespace PsychedelicExperience.Common.Tests
{
    public static class Execute
    {
        /// <summary>
        /// Execute the <see cref="action" /> until <see cref="acceptanceCriteria" /> is met with in interval of 2 seconds. 
        /// When the criteria is not met after 10 seconds the execution will fail.
        /// </summary>
        public static T WithTimeOut<T>(Func<T> action, Func<T, bool> acceptanceCriteria, Func<T, string> errorMessageConstructor)
        {
            return WithTimeOut(TimeSpan.FromSeconds(10), action, acceptanceCriteria, errorMessageConstructor, TimeSpan.FromSeconds(2));
        }

        public static T WithTimeOut<T>(Func<Task<T>> action, Func<T, bool> acceptanceCriteria, Func<T, string> errorMessageConstructor)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return WithTimeOut(TimeSpan.FromSeconds(10), () => action().GetResult(), acceptanceCriteria, errorMessageConstructor, TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Execute the <see cref="action" /> until <see cref="acceptanceCriteria" /> is met with in interval of 2 seconds. 
        /// When the criteria is not met after <see cref="timeOut"/> the execution will fail.
        /// </summary>
        public static T WithTimeOut<T>(TimeSpan timeOut, Func<T> action, Func<T, bool> acceptanceCriteria, Func<T, string> errorMessageConstructor)
        {
            return WithTimeOut(timeOut, action, acceptanceCriteria, errorMessageConstructor, TimeSpan.FromSeconds(2));
        }

        private static T WithTimeOut<T>(TimeSpan timeOut, Func<T> action, Func<T, bool> acceptanceCriteria, Func<T, string> errorMessageConstructor, TimeSpan interval)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (acceptanceCriteria == null) throw new ArgumentNullException(nameof(acceptanceCriteria));
            if (errorMessageConstructor == null) throw new ArgumentNullException(nameof(errorMessageConstructor));
            if (timeOut.TotalMilliseconds <= 0) throw new ArgumentException("Number of TotalMilliseconds of timout value should be > 0", nameof(timeOut));

            var beginTimeStamp = DateTime.Now;
            var state = action();

            while (!acceptanceCriteria(state))
            {
                CheckTimeOut(beginTimeStamp, timeOut, () => errorMessageConstructor(state));

                Thread.Sleep(interval);

                state = action();
            }
            return state;
        }

        private static void CheckTimeOut(DateTime start, TimeSpan timeOut, Func<string> errorMessageConstructor)
        {
            var elapsedMilliseconds = (DateTime.Now - start).TotalMilliseconds;
            if (elapsedMilliseconds > timeOut.TotalMilliseconds)
            {
                throw new InvalidOperationException(errorMessageConstructor());
            }
        }
    }
}
