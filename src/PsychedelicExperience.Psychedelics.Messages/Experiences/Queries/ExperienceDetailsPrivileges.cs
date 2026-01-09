namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class ExperienceDetailsPrivileges
    {
        public FieldPrivilege Title { get; set; }
        public FieldPrivilege DateTime { get; set; }
        public FieldPrivilege Level { get; set; }
        public FieldPrivilege Set { get; set; }
        public FieldPrivilege Setting { get; set; }
        public FieldPrivilege PrivateNotes { get; set; }
        public FieldPrivilege PublicDescription { get; set; }
        public FieldPrivilege PrivacyLevel { get; set; }
        public FieldPrivilege Tags { get; set; }
        public FieldPrivilege Doses { get; set; }

        public bool IsOwner { get; set; }
        public bool Editable { get; set; }
        public bool Remove { get; set; }
        public bool Report { get; set; }
    }
}