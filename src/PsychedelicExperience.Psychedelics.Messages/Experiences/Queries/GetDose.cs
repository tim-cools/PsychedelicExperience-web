using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class GetDose : IRequest<Dose>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }

        public GetDose(UserId userId, DoseId doseId)
        {
            UserId = userId;
            DoseId = doseId;
        }
    }
}
