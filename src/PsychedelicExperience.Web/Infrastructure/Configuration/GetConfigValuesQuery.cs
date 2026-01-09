using System.Collections.Generic;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Web.Infrastructure.Configuration
{
    public class GetConfigValuesQuery : IRequest<IDictionary<string, object>>
    {
    }
}