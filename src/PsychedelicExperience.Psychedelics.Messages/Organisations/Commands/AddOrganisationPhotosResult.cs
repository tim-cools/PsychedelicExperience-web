using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class AddOrganisationPhotosResult : Result
    {
        public Guid[] Photos { get; }

        public AddOrganisationPhotosResult()
        {
        }

        public AddOrganisationPhotosResult(bool success, params ValidationError[] errors) : base(success, errors)
        {
        }

        public AddOrganisationPhotosResult(Guid[] photos) : base(true)
        {
            Photos = photos;
        }
    }
}