using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events
{
    public class Image
    {
        public ImageId Id { get; }
        public UserId UserId { get; }
        public string FileName { get; }
        public string OriginalFileName { get; }

        public Image(ImageId id, UserId userId, string fileName, string originalFileName)
        {
            Id = id;
            FileName = fileName;
            OriginalFileName = originalFileName;
            UserId = userId;
        }
    }
}