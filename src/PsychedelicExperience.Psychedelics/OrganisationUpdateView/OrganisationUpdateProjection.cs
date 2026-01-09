using System;
using Marten.Events.Projections;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdateView
{
    public class OrganisationUpdateProjection : ViewProjection<OrganisationUpdate, Guid>
    {
        public OrganisationUpdateProjection()
        {
            ProjectEvent<OrganisationUpdateAdded>(Project);
            ProjectEvent<OrganisationUpdateRemoved>(Project);
            ProjectEvent<OrganisationUpdateSubjectChanged>(Project);
            ProjectEvent<OrganisationUpdateContentChanged>(Project);
            ProjectEvent<OrganisationUpdatePrivacyChanged>(Project);
        }

        private void Project(OrganisationUpdate view, OrganisationUpdateAdded @event)
        {
            view.OrganisationId = (Guid) @event.OrganisationId;

            view.Subject = @event.Subject;
            view.Content = @event.Content;
            view.Privacy = (OrganisationUpdatePrivacy)@event.Privacy;

            view.CreatedBy = @event.UserId;
            view.Created = @event.EventTimestamp;

            view.LastUpdatedBy = @event.UserId;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(OrganisationUpdate view, OrganisationUpdateSubjectChanged @event)
        {
            view.Subject = @event.Subject;
            view.LastUpdatedBy = @event.UserId;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(OrganisationUpdate view, OrganisationUpdateContentChanged @event)
        {
            view.Content = @event.Content;
            view.LastUpdatedBy = @event.UserId;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(OrganisationUpdate view, OrganisationUpdatePrivacyChanged @event)
        {
            view.Privacy = (OrganisationUpdatePrivacy) @event.Privacy;
            view.LastUpdatedBy = @event.UserId;
            view.LastUpdated = @event.EventTimestamp;
        }

        private void Project(OrganisationUpdate view, OrganisationUpdateRemoved @event)
        {
            view.Removed = true;
            view.LastUpdatedBy = @event.UserId;
            view.LastUpdated = @event.EventTimestamp;
        }
    }
}