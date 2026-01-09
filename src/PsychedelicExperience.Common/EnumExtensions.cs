using System;

namespace PsychedelicExperience.Common
{
    public static class EnumExtensions
    {
        public static TDestination CastByName<TDestination>(this Enum value) 
            where TDestination : struct
        {
            if (!Enum.TryParse<TDestination>(value.ToString(), out var result))
            {
                throw new InvalidOperationException($"Could not cast enum value: {value} to {typeof(TDestination).FullName}");
            }

            return result;
        }
    }
}