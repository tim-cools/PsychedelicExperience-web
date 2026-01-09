using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;

namespace PsychedelicExperience.Web.ViewModels.Views
{
    public class OrganisationsDetailState
    {
        public OrganisationDetails Data { get; set; }

        public OrganisationReviewResult ReviewData { get; set; }
        public OrganisationUpdateDetails UpdateData { get; set; }
        public OrganisationUpdatesResult Updates { get; set; }

        public bool PreFilled { get; set; }
    }
}