using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PsychedelicExperience.Web.Infrastructure.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class UnauthenticateWhenSessionExpired : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var notAuthenticated = !filterContext.HttpContext.User.Identity.IsAuthenticated;
            var hasAuthorizationHeader = request.Headers.ContainsKey("Authorization");

            if (hasAuthorizationHeader && notAuthenticated)
            {
                filterContext.Result = new UnauthorizedResult();
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
