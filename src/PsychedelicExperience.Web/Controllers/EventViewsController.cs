using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Resources;

namespace PsychedelicExperience.Web.Controllers
{
    public class EventViewsController : ViewController
    {
        public EventViewsController(IMediator messageDispatcher, IConfiguration configuration)
            : base(messageDispatcher, configuration)
        {
            if (messageDispatcher == null) throw new ArgumentNullException(nameof(messageDispatcher));
        }

        [HttpGet("~/event/type/{eventType}")]
        [HttpGet("~/event/type/{eventType}/page/{page}")]
        public async Task<IActionResult> GetByType(int page = 0 , string country = null, OrganisationId organisationId = null , EventType? eventType = null, string tag = null, string query = null, bool filterByUser = false)
        {
            var getEvents = new GetEvents(null, eventType, country, tag != null ? new[]{ tag} : null, query, page, filterByUser, organisationId);

            return await ViewWithState(getEvents, (builder, result) => builder.WithEvents(result));
        }

        [HttpGet("~/event/country/{country}")]
        [HttpGet("~/event/country/{country}/page/{page}")]
        public async Task<IActionResult> GetByCountry(int page = 0 , string country = null)
        {
            var getEvents = new GetEvents(null, null, country, null, null, page);

            return await ViewWithState(getEvents, (builder, result) => builder.WithEvents(result));
        }

        [HttpGet("~/event/country/{country}/type/{eventType}")]
        [HttpGet("~/event/country/{country}/type/{eventType}/page/{page}")]
        public async Task<IActionResult> GetByCountryAndType(int page = 0 , string country = null, EventType? eventType = null)
        {
            var getEvents = new GetEvents(null, eventType, country, null, null, page);

            return await ViewWithState(getEvents, (builder, result) => builder.WithEvents(result));
        }

        [HttpGet("~/event")]
        [HttpGet("~/event/page/{page}")]

        [HttpGet("~/event/tag/{tag}")]
        [HttpGet("~/event/tag/{tag}/page/{page}")]


        [HttpGet("~/event/type/{eventType}/tag/{tag}")]
        [HttpGet("~/event/type/{eventType}/tag/{tag}/page/{page}")]

        [HttpGet("~/event/organisation/{organisationId}")]
        [HttpGet("~/event/organisation/{organisationId}/page/{page}")]

        [HttpGet("~/event/organisation/{organisationId}/tag/{tag}")]
        [HttpGet("~/event/organisation/{organisationId}/tag/{tag}/page/{page}")]

        [HttpGet("~/event/organisation/{organisationId}/type/{eventType}")]
        [HttpGet("~/event/organisation/{organisationId}/type/{eventType}/page/{page}")]

        [HttpGet("~/event/organisation/{organisationId}/type/{eventType}/tag/{tag}")]
        [HttpGet("~/event/organisation/{organisationId}/type/{eventType}/tag/{tag}/page/{page}")]

        [HttpGet("~/event/country/{country}/tag/{tag}")]
        [HttpGet("~/event/country/{country}/tag/{tag}/page/{page}")]

        [HttpGet("~/event/country/{country}/type/{eventType}/tag/{tag}")]
        [HttpGet("~/event/country/{country}/type/{eventType}/tag/{tag}/page/{page}")]

        [HttpGet("~/event/country/{country}/organisation/{organisationId}")]
        [HttpGet("~/event/country/{country}/organisation/{organisationId}/page/{page}")]

        [HttpGet("~/event/country/{country}/organisation/{organisationId}/tag/{tag}")]
        [HttpGet("~/event/country/{country}/organisation/{organisationId}/tag/{tag}/page/{page}")]

        [HttpGet("~/event/country/{country}/organisation/{organisationId}/type/{eventType}")]
        [HttpGet("~/event/country/{country}/organisation/{organisationId}/type/{eventType}/page/{page}")]

        [HttpGet("~/event/country/{country}/organisation/{organisationId}/type/{eventType}/tag/{tag}")]
        [HttpGet("~/event/country/{country}/organisation/{organisationId}/type/{eventType}/tag/{tag}/page/{page}")]
        public async Task<IActionResult> Get(int page = 0 , string country = null, OrganisationId organisationId = null , EventType? eventType = null, string tag = null, string query = null, bool filterByUser = false)
        {
            var getEvents = new GetEvents(null, eventType, country, tag != null ? new[]{ tag} : null, query, page, filterByUser, organisationId);

            return await ViewWithState(getEvents, (builder, result) => builder.WithEvents(result));
        }

        [HttpGet("~/event/{id}")]
        public async Task<IActionResult> GetById(EventId id)
        {
            var query = new GetEvent(null, id);


            return await ViewWithState(query,
                (builder, result) => builder.WithEvent(result),
                result => result.RedirectWhen(@event => @event != null, @event => @event.Url, Request.QueryString));
        }

        [HttpGet("~/event/{id}/{slug}")]
        public async Task<IActionResult> GetByIdAndSlug(EventId id, string slug)
        {
            var query = new GetEvent(null, id);

            return await ViewWithState(query, (builder, result) => builder.WithEvent(result));
        }

        [HttpGet("~/event/{id}/edit")]
        public async Task<IActionResult> Edit(EventId id)
        {
            var query = new GetEvent(null, id);

            return await ViewWithState(query, (builder, result) => builder.WithEvent(result));
        }

        [HttpGet("~/event/summaries")]
        public async Task<IActionResult> Summaries(EventId id)
        {
            var getEvents = new GetEvents(null);

            var response = await MessageDispatcher.Send(getEvents);

            return View(response.Events);
        }

        /// <summary>
        /// Get a Image from an event. Content type should be jpg/png.
        /// </summary>
        [HttpGet("~/event/{id}/image")]
        [ProducesResponseType(typeof(File), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Image(EventId id, int height = 400, int width = 750)
        {
            var query = new GetEventImage(id);
            var result = await MessageDispatcher.Send(query);
            if (result == null)
            {
                return width == 180 ? Image(WebResources.DefaultEventImageSmall) : Image(WebResources.DefaultEventImage);
            }

            var folder = Configuration.PhotosFolder();
            var fileName = Path.GetFileName(result.FileName);

            var sourceFile = Path.Combine(folder, fileName);
            if (!System.IO.File.Exists(sourceFile)) return new NotFoundResult();

            var resized = ImageSizes.GetResized(folder, sourceFile, height, width);
            return FileImage(resized ?? fileName);
        }

        private static IActionResult FileImage(string sourceFile)
        {
            var stream = System.IO.File.OpenRead(sourceFile);

            return new FileStreamResult(stream, "image/" + Path.GetExtension(sourceFile).TrimStart('.'));
        }

        private static FileStreamResult Image(byte[] data)
        {
            var stream = new MemoryStream(data);
            return new FileStreamResult(stream, "image/png");
        }
    }
}
