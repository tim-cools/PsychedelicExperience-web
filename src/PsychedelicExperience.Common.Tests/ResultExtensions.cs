using System.Threading.Tasks;
using Shouldly;

namespace PsychedelicExperience.Common.Tests
{
    public static class ResultExtensions
    {
        public static void ShouldBeSuccess(this Result result)
        {
            result.Succeeded.ShouldBeTrue(result.ToString);
        }

        public static T GetResult<T>(this Task<T> task)
        {
            task.Wait(5000);
            return task.Result;
        }
    }
}