using Newtonsoft.Json;
using Shouldly;

namespace PsychedelicExperience.Common.Tests
{
    public static class AssertExtensions
    {
        public static void ShouldBeDeepEqual<T>(this T actual, T expected)
        {
            //very basic deep equal
            var actualJson = JsonConvert.SerializeObject(actual, Formatting.Indented);
            var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);

            actualJson.ShouldBe(expectedJson);
        }
    }
}