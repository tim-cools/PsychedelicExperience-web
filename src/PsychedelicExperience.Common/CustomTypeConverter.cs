using System;
using Newtonsoft.Json;

namespace PsychedelicExperience.Common
{
    public abstract class CustomTypeConverter : JsonConverter
    {
        public abstract Type Type { get; }
    }
}