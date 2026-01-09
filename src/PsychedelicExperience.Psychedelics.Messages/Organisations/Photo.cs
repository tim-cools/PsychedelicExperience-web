using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
    public class Photo
    {
        public PhotoId Id { get; }
        public UserId UserId { get; }
        public string FileName { get; }
        public string OriginalFileName { get; }

        public Photo(PhotoId id, UserId userId, string fileName, string originalFileName)
        {
            Id = id;
            FileName = fileName;
            OriginalFileName = originalFileName;
            UserId = userId;
        }
    }
}