namespace PsychedelicExperience.Web.ViewModels.Views
{
    public class ClientState
    {
        private ServerState _server;
        private UserState _user;
        private ExperiencesListState _experiencesList;
        private ExperiencesDetailState _experiencesDetail;
        private OrganisationsListState _organisationsList;
        private OrganisationsDetailState _organisationsDetail;
        private DocumentsListState _documentsList;
        private DocumentDetailState _documentDetail;
        private EventsListState _eventsList;
        private EventDetailState _eventDetail;

        public ServerState Server => _server ??= new ServerState();

        public UserState User => _user ??= new UserState();

        public ExperiencesListState ExperiencesList => _experiencesList ??= new ExperiencesListState();
        public ExperiencesDetailState ExperiencesDetail => _experiencesDetail ??= new ExperiencesDetailState();

        public OrganisationsListState OrganisationsList => _organisationsList ??= new OrganisationsListState();
        public OrganisationsDetailState OrganisationsDetail => _organisationsDetail ??= new OrganisationsDetailState();

        public DocumentsListState DocumentsList => _documentsList ??= new DocumentsListState();
        public DocumentDetailState DocumentDetail => _documentDetail ??= new DocumentDetailState();

        public EventsListState EventsList => _eventsList ??= new EventsListState();
        public EventDetailState EventDetail => _eventDetail ??= new EventDetailState();
    }
}