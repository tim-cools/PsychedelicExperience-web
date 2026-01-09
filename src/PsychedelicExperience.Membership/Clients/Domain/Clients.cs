using System.Collections.Generic;
using Marten.Schema.Identity;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Security;

namespace PsychedelicExperience.Membership.Clients.Domain
{
    public static class Clients
    {
        public static IEnumerable<Client> Get(IConfiguration configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    Id = CombGuidIdGeneration.NewGuid(),
                    Key = "pex",
                    Name = "Psychedelic Experience Client",
                    ApplicationType = ApplicationTypes.JavaScript,
                    Active = true,
                    AllowedOrigin = configuration.IsProduction() ? configuration.WebHostName() : "*",
                    RedirectUri = configuration.WebUrl("account/authorized")
                },
                new Client
                {
                    Id = CombGuidIdGeneration.NewGuid(),
                    Key = "automatedTests",
                    Secret = Hasher.ComputeSHA256("*automatedTests@localhost"),
                    Name = "Automated tests",
                    ApplicationType = ApplicationTypes.NativeConfidential,
                    Active = true,
                    AllowedOrigin = "*",
                    RedirectUri = "*"
                }
            };
        }
    }
}