using System;
using System.Threading.Tasks;

namespace PsychedelicExperience.Psychedelics.Security
{
    public interface IProtectedRequestProvider
    {
        Task<string> GenerateInvite(Guid organisationId, string email);
        Task<ValidationRequestResult> ValidateInvite(string request, string email);
    }
}