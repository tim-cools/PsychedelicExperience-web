namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class FieldPrivilege
    {
        public bool Visible { get; set; }
        public bool Editable { get; set; }

        public FieldPrivilege()
        {
        }

        public FieldPrivilege(bool visible, bool editable)
        {
            Visible = visible;
            Editable = editable;
        }
    }
}