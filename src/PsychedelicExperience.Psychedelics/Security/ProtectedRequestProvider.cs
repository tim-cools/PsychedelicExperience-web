using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Security
{
    public class ProtectedRequestProvider : IProtectedRequestProvider
    {
        private readonly TimeSpan _tokenLifespan = TimeSpan.FromDays(30);

        private readonly ILogger _logger;
        private readonly IDataProtectionProvider _protectorProvider;

        public ProtectedRequestProvider(ILogger<ProtectedRequestProvider> logger, IDataProtectionProvider protectorProvider)
        {
            _logger = logger;
            _protectorProvider = protectorProvider;
        }

        public virtual Task<string> GenerateInvite(Guid organisationId, string email)
        {
            _logger.LogDebugMethod(nameof(GenerateInvite), new { organisationId, email });

            using (var memoryStream = new MemoryStream())
            using (var writer = memoryStream.CreateWriter())
            {
                writer.Write(DateTimeOffset.UtcNow);
                writer.Write(organisationId.ToString());
                writer.Write(email.ToLowerInvariant());

                var protector = _protectorProvider.CreateProtector("invite-organisation");
                var payload = protector.Protect(memoryStream.ToArray());

                var base64String = Convert.ToBase64String(payload);
                
                return Task.FromResult(base64String);
            }
        }

        public virtual Task<ValidationRequestResult> ValidateInvite(string request, string email)
        {
            _logger.LogDebugMethod(nameof(ValidateInvite), new { email });

            return Task.FromResult(ValidateInviteSync(request, email));
        }

        private ValidationRequestResult ValidateInviteSync(string request, string email)
        {
            try
            {
                var protector = _protectorProvider.CreateProtector("invite-organisation");
                var payload = protector.Unprotect(Convert.FromBase64String(request));

                using (var stream = new MemoryStream(payload))
                using (var reader = stream.CreateReader())
                {
                    var tokenTimestamp = reader.ReadDateTimeOffset();
                    var actualOrganisationId = reader.ReadString();
                    if (!Guid.TryParse(actualOrganisationId, out Guid organisationId))
                    {
                        return ValidationRequestResult.UnknownError;
                    }

                    if (email == null)
                    {
                        return ValidationRequestResult.UserNotLoggedin(organisationId);
                    }

                    if (tokenTimestamp + _tokenLifespan < DateTimeOffset.UtcNow)
                    {
                        return ValidationRequestResult.RequestExpired(organisationId);
                    }

                    var actualEMail = reader.ReadString();
                    if (actualEMail.ToLowerInvariant() != email.ToLowerInvariant())
                    {
                        return ValidationRequestResult.InvalidUser(organisationId);
                    }

                    return ValidationRequestResult.Success(organisationId);
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarningMethod(nameof(ValidateInvite), exception, new {email});
                return ValidationRequestResult.UnknownError;
            }
        }
    }
}