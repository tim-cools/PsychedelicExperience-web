using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace PsychedelicExperience.Web.Infrastructure
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(this IRouteBuilder routeBuilder)
        {
            //routeBuilder.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routeBuilder.MapRoute(name: "index", template: "", defaults: new { controller = "Home", action = "Index" });

            //routeBuilder.MapRoute(
            //    name: "client-experience",
            //    template: "experience",
            //    defaults: new { controller = "Home", action = "Index", id = string.Empty });

            //routeBuilder.MapRoute(

            //    name: "client-experience-by-substance",
            //    template: "experience/substance/{substance}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter. });

            //routeBuilder.MapRoute(
            //    name: "client-experience-by-id",
            //    template: "experience/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = string.Empty });

            routeBuilder.MapRoute(name: "server-account", template: "Account/{action}", defaults: new { controller = "Account" });
        }
    }
}
