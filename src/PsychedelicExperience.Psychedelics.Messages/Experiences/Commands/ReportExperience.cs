using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class ReportExperience : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public string Reason { get; }

        public ReportExperience(ExperienceId experienceId, UserId userId, string reason)
        {
            ExperienceId = experienceId;
            UserId = userId;
            Reason = reason;
        }
    }
}