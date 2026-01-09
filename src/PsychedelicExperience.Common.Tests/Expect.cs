using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Organisations
{
    public class Expect
    {
        public static Exception Exception(Func<Result> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            try
            {
                func();
                throw new InvalidOperationException("Exception expected");
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
    }
}