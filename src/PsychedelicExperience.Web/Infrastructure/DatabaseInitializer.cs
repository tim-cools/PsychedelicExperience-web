using System;
using Microsoft.AspNetCore.Builder;

namespace PsychedelicExperience.Web.Infrastructure
{
    public static class DatabaseInitializer
    {
        public static IApplicationBuilder InitalizeDatabase(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            var messageDispatcher = app.ApplicationServices.GetMessageDispatcher();

            messageDispatcher.Send(new Membership.Messages.Infrastructure.InitializeDatabaseCommand());
            //messageDispatcher.Send(new Psychedelics.Messages.Infrastructure.InitializeDatabaseCommand());

            return app;
        }
    }
}
