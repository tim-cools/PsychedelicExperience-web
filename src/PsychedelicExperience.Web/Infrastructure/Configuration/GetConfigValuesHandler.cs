using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace PsychedelicExperience.Web.Infrastructure.Configuration
{
    public class GetConfigValuesHandler : IAsyncRequestHandler<GetConfigValuesQuery, IDictionary<string, object>>
    {
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public GetConfigValuesHandler(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public Task<IDictionary<string, object>> Handle(GetConfigValuesQuery query)
        {
            var sections = _configuration.GetChildren();
            var values = FormatSections(sections);
            values["HostingEnvironment"] = new Dictionary<string, string>
            {
                {"EnvironmentName", _environment.EnvironmentName},
                {"ApplicationName", _environment.ApplicationName},
                {"ContentRootPath", _environment.ContentRootPath},
                {"WebRootPath", _environment.WebRootPath}
            };
            return Task.FromResult(values);
        }

        private static IDictionary<string, object> FormatSections(IEnumerable<IConfigurationSection> sections)
        {
            var data = new Dictionary<string, object>();
            foreach (var section in sections)
            {
                var children = section.GetChildren().ToArray();
                if (children.Any())
                {
                    data[section.Key] = FormatSections(children);
                }
                else
                {
                    data[section.Key] = section.Value;
                }
            }
            return data;
        }
    }
}