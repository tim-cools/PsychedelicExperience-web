using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
    public class Safety
    {
        public OptionalDescription MedicalFacilitiesOnsite { get; set; }
        public OptionalDescription MedicalFacilitiesNearby { get; set; }
        public OptionalDescription PsychologicalTherapisOnsite { get; set; }
        public OptionalDescription IntakeProcess { get; set; }
        public OptionalDescription IntegrationProcess { get; set; }
    }
}