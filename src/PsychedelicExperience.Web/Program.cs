using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Web.Infrastructure;


namespace PsychedelicExperience.Web
{
    public static class Program
    {
        const string environment = "ASPNETCORE_ENVIRONMENT";

        public static void Main(string[] args)
        {
            SetEnvironment(args);

            var currentDirectory = Directory.GetCurrentDirectory();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseWebRoot(Path.Combine(currentDirectory, "Client", "wwwroot"))
                .UseUrls("http://localhost:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(SetupConfiguration(args))
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build(); 

            host.Run();
        }

        
        private static Action<WebHostBuilderContext, IConfigurationBuilder> SetupConfiguration(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable(environment);

            return (context, builder) => builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.private.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddCommandLine(args.Skip(1).ToArray())
                .AddEnvironmentVariables();
        }

        private static void SetEnvironment(string[] args)
        {
            var index = Array.IndexOf(args, $"--{environment}");
            if (index >= 0)
            {
                Environment.SetEnvironmentVariable(environment, args[index + 1]);
            }
        }
    }
}