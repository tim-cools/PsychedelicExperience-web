using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages;
using PsychedelicExperience.Psychedelics.Messages.Statistics;
using PsychedelicExperience.Psychedelics.Messages.Statistics.Commands;
using PsychedelicExperience.Web.Infrastructure.Configuration;
using PsychedelicExperience.Web.Infrastructure.Security;
using StructureMap.TypeRules;

namespace PsychedelicExperience.Web.Controllers.Api
{
    public class AdminController : ViewController
    {
        //TODO move logic to proper handlers

        private readonly IMailSender _mailSender;
        private readonly IDaemonController _daemon;

        public AdminController(IMailSender mailSender, IDaemonController daemon,
            IMediator messageDispatcher, IConfiguration configuration) : base(messageDispatcher, configuration)
        {
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _daemon = daemon ?? throw new ArgumentNullException(nameof(daemon));
        }

        [Authorize]
        [HttpGet("~/api/admin/diagnostics")]
        public async Task<IActionResult> Diagnostics()
        {
            if (!await IsAdministrator()) return BadRequest();

            var data = new
            {
                Environment = Environment.GetEnvironmentVariables(),
                Request = new
                {
                    Request.Scheme,
                    Request.Host,
                    Request.IsHttps,
                    Request.Path,
                    Request.Headers,
                    Request.QueryString,
                    Request.Cookies
                },
                Version = ApplicationVersion.Get()
            };
            return Json(data);
        }

        [UnauthenticateWhenSessionExpired]
        [HttpGet("~/api/admin/statistics")]
        [ProducesResponseType(typeof(Statistics), 200)]
        public async Task<IActionResult> Statistics(Format format = Format.Json, Category category = Category.All)
        {
            var experiencesQuery = new GetStatistics(format, category);
            var result = await MessageDispatcher.Send(experiencesQuery);

            return result is JsonStatistics _ ? Json(result) :
                result is CsvStatistics csv ? (IActionResult) File(csv.Bytes, "text/csv", csv.FileName) :
                throw new InvalidOperationException($"Invalid result: {result.GetType()}");
        }

        [Authorize]
        [HttpGet("~/api/admin/diagnostics/engine")]
        public async Task<IActionResult> DiagnosticsJs()
        {
            if (!await IsAdministrator()) return BadRequest();

            return View("js-{auto}");
        }

        [Authorize]
        [HttpGet("~/api/admin/projections")]
        public async Task<IActionResult> Projections()
        {
            if (!await IsAdministrator()) return BadRequest();

            var result = new List<object>();
            foreach (var track in _daemon.GetProjections())
            {
                result.Add(new
                {
                    track.ViewType,
                    track.LastEncountered
                });
            }
            return Json(result);
        }

        [Authorize]
        [HttpGet("~/api/admin/projections/rebuild")]
        public async Task<IActionResult> RebuildProjections(CancellationToken token)
        {
            if (!await IsAdministrator()) return BadRequest();

            var result = await _daemon.Rebuild(token);

            return Json(result);
        }

        [Authorize]
        [HttpGet("~/api/admin/projections/{name}/rebuild")]
        public async Task<IActionResult> RebuildProjection(string name, CancellationToken token)
        {
            if (!await IsAdministrator()) return BadRequest();

            var result = await _daemon.Rebuild(name, token);

            return Json(result);
        }

        
        [Authorize]
        [HttpGet("~/api/admin/config/")]
        [ProducesResponseType(typeof(IDictionary<string, object>), 200)]
        public async Task<IActionResult> GetConfig()
        {
            if (!await IsAdministrator()) return BadRequest();

            var data = await MessageDispatcher.Send(new GetConfigValuesQuery());
            return Json(data);
        }

        [HttpPut("~/api/admin/notification/")]
        [ProducesResponseType(typeof(IDictionary<string, object>), 200)]
        public async Task Notification(string text)
        {
            var suffix = Configuration.IsProduction() ? string.Empty : $" [{Configuration.Environment()}]";

            await _mailSender.SendMail($"PsychedelicExperience Support{suffix}", text);
        }
    }

    public class ApplicationVersion
    {
        public string AssemblyFullName { get; }
        public string AssemblyImageRuntimeVersion { get; }

        private ApplicationVersion(string assemblyFullName, string assemblyImageRuntimeVersion)
        {
            AssemblyFullName = assemblyFullName;
            AssemblyImageRuntimeVersion = assemblyImageRuntimeVersion;
        }

        public static ApplicationVersion Get()
        {
            var assembly = typeof(ApplicationVersion).GetAssembly();
            return new ApplicationVersion(assembly.FullName, assembly.ImageRuntimeVersion);
        }
    }
}
