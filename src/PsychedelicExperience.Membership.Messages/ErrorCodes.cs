namespace PsychedelicExperience.Membership.Messages
{
    public class ErrorCodes
    {
        public const string ClientNotRegistered = nameof(ClientNotRegistered);
        public const string ClientInactive = nameof(ClientInactive);
        public const string ClientAuthicationFailed = nameof(ClientAuthicationFailed);

        public const string RedirectUriInvalid = nameof(RedirectUriInvalid);
        public const string RefreshTokenInvalid = nameof(RefreshTokenInvalid);
        public const string RefreshTokenAlreadyExists = nameof(RefreshTokenAlreadyExists);
        public const string UserNotFound = nameof(UserNotFound);

        public static string DuplicateEmail = nameof(DuplicateEmail);
        public static string InvalidPassword = nameof(InvalidPassword);
    }
}