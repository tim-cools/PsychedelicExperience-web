using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Baseline;
using JavaScriptViewEngine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using JavaScriptViewEngine.Pool;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.Services;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Logging;
using PsychedelicExperience.Web.Infrastructure.Security;
using SimpleMvcSitemap;
using StatsdClient;
using StructureMap;
using Swashbuckle.Swagger.Model;
using AuthenticationMiddleware = PsychedelicExperience.Web.Infrastructure.Security.AuthenticationMiddleware;

namespace PsychedelicExperience.Web
{
    public class Startup
    {
        private const string DefaultName = "default";

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services
                .AddIdentity<User, Role>()
                .AddDefaultTokenProviders();

            services
                .AddLogging(LoggingConfiguration.Configure(_configuration, _environment))
                .AddMemoryCache()
                .Configure<KestrelServerOptions>(options => { options.AddServerHeader = false; })
                .AddAuthentication(_configuration);
            
            services.AddMvc(options =>
                {
                    options.ModelBinderProviders.Insert(0, new CustomBindingProvider());
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.Converters.AddRange(CustomJsonConverters());
                });

            services.ConfigureApplicationCookie(options => {
                options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied);
                options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin);
            });

            //AddSwagger(services);
            AddJsViewEngine(services);
            AddSimpleSiteMap(services);

            services
                .Configure<MailOptions>(_configuration.GetSection("mailOptions"))                
                .AddCors(ConfigureCors)
                .AddLogging()
                .AddAndConfigureDataProtection();

            return CreateContainerServiceProvider(services);
        }

        private static IList<JsonConverter> CustomJsonConverters()
        {
            return PsychedelicsRegistry.CustomTypeConverters
                .Union(MembershipRegistry.CustomTypeConverters)
                .Union(CommonRegistry.CustomTypeConverters)
                .Select(type => Activator.CreateInstance(type) as JsonConverter)
                .ToList();
        }

        private void AddSimpleSiteMap(IServiceCollection services)
        {
            services.TryAddTransient<ISitemapProvider, SitemapProvider>();
        }

        private void AddJsViewEngine(IServiceCollection services)
        {
            services.AddJsEngine(builder =>
            {
                builder.UseNodeRenderEngine(options =>
                {
                    options.EnvironmentVariables = new Dictionary<string, string>
                    {
                        {"environment", _configuration.Environment()},
                        {"PEX_API_URL", _configuration.WebHostName().TrimEnd('/')}
                    };
                    var serverScript = Path.Combine("scripts", "load-server");


                    options.ProjectDirectory = _environment.WebRootPath;
                    options.GetModuleName = (path, model, viewBag, routeValues, area, viewType) => area == DefaultName
                        ? serverScript
                        : area;
                });
                builder.UseSingletonEngineFactory();
            });

            services.Configure<RenderPoolOptions>(options =>
            {
                options.WatchPath = Path.Combine(_environment.WebRootPath, "..\\src");
                options.WatchFiles = new List<string>
                {
                    Path.Combine(options.WatchPath, "vendors.js"),
                    Path.Combine(options.WatchPath, "vendors-ui.js"),
                    Path.Combine(options.WatchPath, "server.js"),
                    Path.Combine(options.WatchPath, "load-server.js"),
                };
            });
        }

        private bool SwaggerEnabled()
        {
            return false;
            var fileName = Path.Combine(_environment.ContentRootPath, _configuration.SwaggerPath());
            return File.Exists(fileName);
        }

        private void AddSwagger(IServiceCollection services)
        {
            var fileName = Path.Combine(_environment.ContentRootPath, _configuration.SwaggerPath());

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Psychedelic Experience API",
                    Description = "Client API ",
                    TermsOfService = "Restricted to"
                });
                if (File.Exists(fileName))
                {
                    options.IncludeXmlComments(fileName);
                }
                
                options.CustomSchemaIds(type => type.FullName);
                options.DescribeAllEnumsAsStrings();
            });
        }

        private void ConfigureCors(CorsOptions options)
        {
            if (_configuration.IsProduction())
            {
                options.AddPolicy(DefaultName, policy =>
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(
                            "http://localhost",
                            "https://d20zb874ut57lk.cloudfront.net",
                            "https://dytky5wb8xu2e.cloudfront.net",
                            "https://staging.psychedelicexperience.net",
                            "https://www.psychedelicexperience.net")
                        .WithExposedHeaders("location"));
                return;
            }

            options.AddPolicy(DefaultName, policy =>
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .WithExposedHeaders("location"));
        }

        private IServiceProvider CreateContainerServiceProvider(IServiceCollection services)
        {
            var container = new Container(configuration =>
            {
                configuration.For<IConfiguration>().Use(_configuration);

                configuration.AddRegistry<WebRegistry>();
                configuration.AddRegistry<ProductionCommonRegistry>();
                configuration.AddRegistry<MembershipRegistry>();
                configuration.AddRegistry<PsychedelicsRegistry>();
            });

            container.Populate(services);

            StartServices(container);

            return container.GetInstance<IServiceProvider>();
        }

        private static void StartServices(Container container)
        {
            var startables = container.GetAllInstances<IStartable>();
            startables.Each(startable => startable.Start());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (env == null) throw new ArgumentNullException(nameof(env));
            
            if (_configuration.GeneralConfigured())
            {
                app.InitalizeDatabase();

                var controller = app.ApplicationServices.GetService<IDaemonController>();
                controller.Start();
            }

            ConfigureWebApp(app, env);
            ConfigureJsonNet();
            ConfigureStatsDogd();
        }

        private void ConfigureStatsDogd()
        {
            var dogstatsdConfig = new StatsdConfig
            {
                StatsdServerName = "localhost",
                StatsdPort = 8125,
                Prefix = "pex",
            };

            DogStatsd.Configure(dogstatsdConfig);
        }

        private void ConfigureJsonNet()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = CustomJsonConverters()
            };
        }

        private void ConfigureWebApp(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions());
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app .UseCors(DefaultName)
                .UseForwardedHeaders(ForwardLoadBalancerHttpHeaders())
                .UseLoggingHandler();

            if (SwaggerEnabled())
            {
                app.UseSwagger();
                app.UseSwaggerUi();
            }

            app.UseMiddleware<AuthenticationMiddleware>()
                .UseStaticFiles();
                //.UseWebSockets()
                //.UseSignalR(app.ApplicationServices)

            //app.UseStatusCodePagesWithReExecute("/Status/Status/{0}");
            app //.UseMiddleware<LogResponseMiddleware>()
                .UseMiddleware<LogRequestMiddleware>()
                .UseJsEngine();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Use((context, next) =>
            {
                context.Response.StatusCode = 404;
                return next();
            });
        }

        private static ForwardedHeadersOptions ForwardLoadBalancerHttpHeaders()
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All,
                RequireHeaderSymmetry = false,
            };

            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            return options;
        }
        static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
            context => {
                if (!context.Request.Path.StartsWithSegments("/api"))
                {
                    return existingRedirector(context);
                }

                context.Response.StatusCode = (int)statusCode;
                return Task.CompletedTask;
            };
    }
}

