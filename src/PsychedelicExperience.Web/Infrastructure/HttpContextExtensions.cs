using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using PsychedelicExperience.Common;
using PsychedelicExperience.Web.Controllers;

namespace PsychedelicExperience.Web.Infrastructure
{
    public static class WebExtensions
    {
        public static async Task<bool> IsProviderSupported(this IAuthenticationSchemeProvider schemeProvider, string provider)
        {
            if (schemeProvider == null) throw new ArgumentNullException(nameof(schemeProvider));
            
            return (await schemeProvider.GetAllSchemesAsync())
                .Any(description => string.Equals(description.DisplayName, provider, StringComparison.OrdinalIgnoreCase));
        }

        public static IActionResult Result(this ControllerBase controller, Result result)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            return result != null && result.Succeeded
                ? controller.Ok()
                : (IActionResult)controller.BadRequest(result);
        }

        public static IActionResult ResultData(this ControllerBase controller, Result result)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            return result.Succeeded
                ? controller.Ok(result)
                : (IActionResult)controller.BadRequest(result);
        }
    }
}