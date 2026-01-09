//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Infrastructure;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.DataProtection;
//using Microsoft.Owin.Builder;
//using Owin;

//namespace PsychedelicExperience.Web.Infrastructure
//{
//    using AppFunc = Func<IDictionary<string, object>, Task>;

//    public static class SignalRExtensions
//    {
//        private static IApplicationBuilder UseAppBuilder(
//            this IApplicationBuilder app,
//            Action<IAppBuilder> configure)
//        {
//            app.UseOwin(addToPipeline =>
//            {
//                addToPipeline(next =>
//                {
//                    var appBuilder = new AppBuilder();
//                    appBuilder.Properties["builder.DefaultApp"] = next;

//                    configure(appBuilder);

//                    return appBuilder.Build<AppFunc>();
//                });
//            });

//            return app;
//        }

//        public static IApplicationBuilder UseSignalR(this IApplicationBuilder app, IServiceProvider applicationServices)
//        {
//            if (app == null) throw new ArgumentNullException(nameof(app));

//            var configuration = new HubConfiguration { EnableDetailedErrors = true };

//            configuration.Resolver.Register(typeof(IProtectedData), () => applicationServices.GetService(typeof(ProtectedDataAdapter)));

//            app.UseAppBuilder(appBuilder => appBuilder.MapSignalR(configuration));

//            return app;
//        }
//    }

//    public class ProtectedDataAdapter : IProtectedData
//    {
//        private readonly IDataProtectionProvider _provider;

//        public ProtectedDataAdapter(IDataProtectionProvider provider)
//        {
//            _provider = provider;
//        }

//        public string Protect(string data, string purpose)
//        {
//            var protector = _provider.CreateProtector(purpose);
//            return protector.Protect(data);
//        }

//        public string Unprotect(string protectedValue, string purpose)
//        {
//            var protector = _provider.CreateProtector(purpose);
//            return protector.Unprotect(protectedValue);
//        }
//    }
//}