using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Membership.Security
{
    public interface IUserDataProtector
    {
        EncryptedString Encrypt(UserId userId, string value);
        string Decrypt(UserId userId, EncryptedString value);
    }
}