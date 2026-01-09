namespace PsychedelicExperience.Psychedelics.TopicInteractionView.Events
{
    public class TopicInteractionUpdated
    {
        public TopicInteraction View { get; set; }

        public TopicInteractionUpdated(TopicInteraction view)
        {
            View = view;
        }
    }
}
