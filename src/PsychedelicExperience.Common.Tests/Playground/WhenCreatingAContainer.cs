using System;
using System.Diagnostics;
using Xunit;

namespace PsychedelicExperience.Common.Tests.Playground
{
    public class WhenConvertingAShortGuid
    {
        [Fact]
        public void ThenItShouldConverBack()
        {
            var value = new ShortGuid("CJZpAZGCp0qvLOtKmbtIEw");
            var message = value.Guid.ToString();
            Debug.WriteLine(message);
        }
    }
}