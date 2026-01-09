using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateExperienceDateTime : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public DateTime? DateTime { get; }

        public UpdateExperienceDateTime(ExperienceId experienceId, UserId userId, DateTime? dateTime)
        {
            ExperienceId = experienceId;
            UserId = userId;
            DateTime = dateTime;
        }
    }
}