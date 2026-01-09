using System;

namespace PsychedelicExperience.Common.Aggregates
{
    public interface IIdentifyable
    {
        Guid Id { get; }
    }
}