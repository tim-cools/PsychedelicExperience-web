using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;
using EventId = PsychedelicExperience.Psychedelics.Messages.Events.EventId;

namespace PsychedelicExperience.Web.Controllers.Api
{
    [UnauthenticateWhenSessionExpired]
    public class OrganisationController : Controller
    {
        private readonly ILogger<EventController> _logger;
        private readonly IMediator _messageDispatcher;
        private readonly IConfiguration _configuration;

        public OrganisationController(IMediator messageDispatcher, IConfiguration configuration, ILogger<EventController> logger)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("~/api/organisation/")]
        [ProducesResponseType(typeof(OrganisationsResult), 200)]
        public async Task<IActionResult> GetList(string query, string country, string[] tags, int page = 0, bool onlyWithoutTags = false, bool filterByUser = false, string[] types = null, bool? hasOwner = null, Format format = Format.Json)
        {
            var userId = User.GetUserId();

            var organisationsQuery = new GetOrganisations(userId, country, query, tags, page, onlyWithoutTags, filterByUser, types, hasOwner, format);
            var result = await _messageDispatcher.Send(organisationsQuery);

            return result is JsonOrganisationsResult _ ? Json(result) :
                result is CsvOrganisationsResult csv ? (IActionResult) File(csv.Bytes, "text/csv", csv.FileName) :
                throw new InvalidOperationException($"Invalid result: {result.GetType()}");
        }

        [HttpGet("~/api/organisation/map")] 
        [ProducesResponseType(typeof(OrganisationsMapResult), 200)]
        public async Task<IActionResult> Map(string[] tags, string country)
        {
            var userId = User.GetUserId();

            var organisationsQuery = new GetOrganisationsMap(userId, country, tags);
            var organisations = await _messageDispatcher.Send(organisationsQuery);

            return Json(organisations);
        }

        [HttpGet("~/api/organisation/{id}/")]
        [ProducesResponseType(typeof(OrganisationDetails), 200)]
        public async Task<IActionResult> Get(OrganisationId id)
        {
            var userId = User.GetUserId();

            var query = new GetOrganisation(userId, id);
            var experience = await _messageDispatcher.Send(query);

            return experience != null ? Json(experience) : id.AcceptedWhenRecent();
        }

        [HttpGet("~/api/organisation/{id}/summary")]
        [ProducesResponseType(typeof(OrganisationSummary), 200)]
        public async Task<IActionResult> Summary(OrganisationId id)
        {
            var userId = User.GetUserId();

            var query = new GetOrganisationSummary(userId, id);
            var organisation = await _messageDispatcher.Send(query);

            return View(organisation);
        }

        [Authorize]
        [HttpPost("~/api/organisation/")]
        public async Task<IActionResult> Post(PostOrganisationRequest request)
        {
            var id = OrganisationId.New();
            var userId = User.GetAuthorizedUserId();

            var command = request.ToCommand(id, userId);

            var result = await _messageDispatcher.Send(command);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return CreatedAtAction("Get", new { id = new ShortGuid(id.Value) }, null);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Remove(OrganisationId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisation(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        /*
        [HttpDelete("~/api/organisation/remove-organisations")]
        [ProducesResponseType(typeof(OrganisationsResult), 200)]
        public async Task<IActionResult> DeleteList(bool confirm = false)
        {
            var userId = User.GetUserId();

            var organisationsQuery = new RemoveOrganisations(userId, confirm);
            var organisations = await _messageDispatcher.Send(organisationsQuery);

            return Json(organisations);
        }
        */

        [HttpPut("~/api/organisation/{id}/report")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Report(OrganisationId id, string reason)
        {
            var userId = User.GetUserId();

            var query = new ReportOrganisation(id, userId, reason);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/update-name")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateTitle(OrganisationId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationName(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/update-description")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateDescription(OrganisationId id, string description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationDescription(id, userId, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/organisation")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> SetOrganisation(OrganisationId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationPerson(id, userId, false);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/person")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> SetPerson(OrganisationId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationPerson(id, userId, true);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/center")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateCenter(OrganisationId id, Center center)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationCenter(id, userId, center);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/practitioner")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdatePractitioner(OrganisationId id, Practitioner practitioner)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationPractitioner(id, userId, practitioner);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/community")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateCommunity(OrganisationId id, Community community)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationCommunity(id, userId, community);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/healthcare-provider")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateHealthcareProvider(OrganisationId id, HealthcareProvider healthcareProvider)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationHealthcareProvider(id, userId, healthcareProvider);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/center")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> DeleteCenter(OrganisationId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationCenter(id, userId, null);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/community")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> DeleteCommunity(OrganisationId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationCommunity(id, userId, null);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/healthcare-provider")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> DeleterHealthcareProvider(OrganisationId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationHealthcareProvider(id, userId, null);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/update-address")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateAddress(OrganisationId id, GooglePlace address)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeOrganisationAddress(id, userId, address.ToAddress());
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/organisation/{id}/contact/{type}/{*value}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> PutContact(OrganisationId id, string type, string value)
        {
            var userId = User.GetAuthorizedUserId();

            var url = Encoding.UTF8.GetString(Convert.FromBase64String(value));

            var command = new AddOrganisationContact(id, userId, type, url);
            var result = await _messageDispatcher.Send(command);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return this.Result(result);
        }

        [HttpPost("~/api/organisation/{id}/contact")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Contact(OrganisationId id, string email, string subject, string message)
        {
            var userId = User.GetUserId();

            var query = new ContactOrganisation(id, userId, email, subject, message);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/contact/{type}/{*value}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveContact(OrganisationId id, string type, string value)
        {
            var userId = User.GetAuthorizedUserId();

            var url = Encoding.UTF8.GetString(Convert.FromBase64String(value));

            var query = new RemoveOrganisationContact(id, userId, type, url);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/warning")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateWarning(OrganisationId id, string title, string content)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new SetOrganisationWarning(id, userId, title, content);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/warning")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> DeleteWarning(OrganisationId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisationWarning(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/info")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateInfo(OrganisationId id, string title, string content)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new SetOrganisationInfo(id, userId, title, content);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/info")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> DeleteInfo(OrganisationId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisationInfo(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/organisation/{id}/tags/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddTag(OrganisationId id, string name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new AddOrganisationTag(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/tags/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveTag(OrganisationId id, string name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisationTag(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/organisation/{id}/types/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddType(OrganisationId id, string name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new AddOrganisationType(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/types/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveType(OrganisationId id, string name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisationType(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/link/{target}/{relation}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Link(OrganisationId id, OrganisationId target, string relation)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new LinkOrganisation(id, target, userId, relation);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/link/{target}/{relation}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Unlink(OrganisationId id, OrganisationId target, string relation)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UnlinkOrganisation(id, target, userId, relation);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/organisation/{id}/owners/{email}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddOwner(OrganisationId id, EMail email)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new AddOrganisationOwner(id, userId, email, null);
            var result = await _messageDispatcher.Send(query);

            return CreatedAtAction("GetOwner", new { id = new ShortGuid(id.Value), ownerId = new ShortGuid(result.OwnerId) }, null);
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        [Authorize]
        [HttpPost("~/api/organisation/{id}/owners/{ownerId}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public Task<IActionResult> GetOwner(OrganisationId id, UserId ownerId)
        {
            throw new NotSupportedException(); //don't remove, this is needed in AddOwner...
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/owners/{ownerId}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveOwner(OrganisationId id, UserId ownerId)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisationOwner(id, userId, ownerId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        /// <summary>
        /// Add photos to an organisation.
        /// </summary>
        [HttpPost("~/api/organisation/{id}/photos/")]
        [ProducesResponseType(typeof(AddOrganisationPhotosResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddPhotos(OrganisationId id, ICollection<IFormFile> files)
        {
            var userId = User.GetUserId();

            var formFiles = await files.Save(_configuration);

            var photos = formFiles
                .Select(file => new Photo(new PhotoId(file.Id), userId, file.FileName, file.OriginalFileName))
                .ToArray();

            if (photos.Length == 0)
            {
                return BadRequest();
            }

            var query = new AddOrganisationPhotos(id, userId, photos);
            var result = await _messageDispatcher.Send(query);

            return this.ResultData(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/photos/{photoId}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemovePhoto(OrganisationId id, PhotoId photoId)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisationPhoto(id, userId, photoId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/organisation/{id}/review")]
        [ProducesResponseType(typeof(CreatedAtActionResult), 201)]  
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> PostReview(OrganisationId id, PostReviewRequest request)
        {
            var organisationReviewId = OrganisationReviewId.New();
            var userId = User.GetAuthorizedUserId();

            var command = request.ToCommand(id, organisationReviewId, userId);
            var result = await _messageDispatcher.Send(command);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return CreatedAtAction("Get", "OrganisationReview", new { organisationId = new ShortGuid(id.Value), reviewId = new ShortGuid(organisationReviewId.Value) }, null);
        }

        [HttpGet("~/api/organisation/tags/")]
        [ProducesResponseType(typeof(Tag[]), 200)]
        public async Task<IActionResult> Tags(string query)
        {
            var userId = User.GetUserId();

            var request = new TagsQuery(userId, query, TagsDomain.Organisations);
            var tags = await _messageDispatcher.Send(request);

            return Json(tags);
        }

        [HttpGet("~/api/organisation/countries/")]
        [ProducesResponseType(typeof(Country[]), 200)]
        public async Task<IActionResult> Countries(string query)
        {
            var userId = User.GetUserId();

            var request = new GetOrganisationCountries(userId, query);
            var countries = await _messageDispatcher.Send(request);

            return Json(countries);
        }
        
        [HttpGet("~/api/organisation/types")]
        [ProducesResponseType(typeof(Tag[]), 200)]
        public async Task<IActionResult> Types()
        {
            var userId = User.GetUserId();

            var request = new TagsQuery(userId, string.Empty, TagsDomain.OrganisationTypes);
            var tags = await _messageDispatcher.Send(request);

            return Json(tags);
        }

        [Authorize]
        [HttpPost("~/api/organisation/{id}/event")]
        [ProducesResponseType(typeof(CreatedAtActionResult), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> PostEvent(OrganisationId id, PostEventRequest request)
        {
            var eventId = EventId.New();
            var userId = User.GetAuthorizedUserId();

            var command = request.ToCommand(id, eventId, userId);
            var result = await _messageDispatcher.Send(command);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(error => error.Message).ToArray());
                _logger.LogWarningMethod(nameof(PostEvent), new { errors, request });

                return BadRequest(result.Errors);
            }

            return CreatedAtAction("Get", "Event", new { id = new ShortGuid(eventId.Value) }, null);
        }

        [HttpGet("~/api/organisation/{id}/event")]
        [ProducesResponseType(typeof(CreatedAtActionResult), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> GetEvent(OrganisationId id)
        {
            var userId = User.GetUserId();

            var organisationsQuery = new GetEvents(userId, organisationId: id);
            var organisations = await _messageDispatcher.Send(organisationsQuery);

            return Json(organisations);
        }

        [Authorize]
        [HttpGet("~/api/organisation/hubspot/{id}")]
        [ProducesResponseType(typeof(CreatedAtActionResult), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Hubspot(long id)
        {
            var userId = User.GetUserId();

            var organisationsQuery = new GetHubspotDefaults(userId, id);
            var organisations = await _messageDispatcher.Send(organisationsQuery);

            return Json(organisations);
        }
    }
}
