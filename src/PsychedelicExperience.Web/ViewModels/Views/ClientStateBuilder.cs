using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;

namespace PsychedelicExperience.Web.ViewModels.Views
{
    public class ClientStateBuilder
    {
        private readonly ClientState _clientState = new ClientState();

        public ClientStateBuilder()
        {
            _clientState.User.Status = "notAuthenticated";
        }

        public ClientStateBuilder WithRouting(string pathName, IConfiguration configuration)
        {
            _clientState.Server.Path = pathName;
            _clientState.Server.Environment = configuration.Environment();
            return this;
        }

        public ClientStateBuilder WithExperiences(ExperiencesResult experiences)
        {
            if (experiences != null)
            {
                _clientState.ExperiencesList.Data = experiences;
                _clientState.ExperiencesList.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithExperience(ExperienceDetails experience)
        {
            if (experience != null)
            {
                _clientState.ExperiencesDetail.Data = experience;
                _clientState.ExperiencesDetail.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithOrganisations(OrganisationsResult organisations)
        {
            if (organisations != null)
            {
                _clientState.OrganisationsList.Data = organisations;
                _clientState.OrganisationsList.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithOrganisation(OrganisationDetails organisation)
        {
            if (organisation != null)
            {
                _clientState.OrganisationsDetail.Data = organisation;
                _clientState.OrganisationsDetail.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithOrganisationReview(OrganisationReviewResult review)
        {
            if (review != null)
            {
                _clientState.OrganisationsDetail.ReviewData = review;
                _clientState.OrganisationsDetail.PreFilled = true;
            }
            return this;
        }


        public ClientStateBuilder WithDocuments(DocumentsResult documents)
        {
            if (documents != null)
            {
                _clientState.DocumentsList.Data = documents;
                _clientState.DocumentsList.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithDocument(DocumentDetails document)
        {
            if (document != null)
            {
                _clientState.DocumentDetail.Data = document;
                _clientState.DocumentDetail.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithOrganisationUpdate(OrganisationUpdateResult update)
        {
            if (update != null)
            {
                _clientState.OrganisationsDetail.Data = update.Organisation;
                _clientState.OrganisationsDetail.UpdateData = update.Update;
                _clientState.OrganisationsDetail.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithOrganisationUpdates(OrganisationUpdatesResult result)
        {
            if (result != null)
            {
                _clientState.OrganisationsDetail.Data = result.Organisation;
                _clientState.OrganisationsDetail.Updates = result;
                _clientState.OrganisationsDetail.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithEvent(EventDetails result)
        {
            if (result != null)
            {
                _clientState.EventDetail.Data = result;
                _clientState.EventDetail.PreFilled = true;
            }
            return this;
        }

        public ClientStateBuilder WithEvents(EventsResult result)
        {
            if (result != null)
            {
                _clientState.EventsList.Data = result;
                _clientState.EventsList.PreFilled = true;
            }
            return this;
        }

        public ClientState Build()
        {
            return _clientState;
        }
    }
}