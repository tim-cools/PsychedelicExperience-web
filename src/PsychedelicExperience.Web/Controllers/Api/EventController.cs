using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;
using EventId = PsychedelicExperience.Psychedelics.Messages.Events.EventId;

namespace PsychedelicExperience.Web.Controllers.Api
{
    [UnauthenticateWhenSessionExpired]
    public class EventController : Controller
    {
        private readonly IMediator _messageDispatcher;
        private readonly IConfiguration _configuration;

        public EventController(IMediator messageDispatcher, IConfiguration configuration)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("~/api/event/")]
        [ProducesResponseType(typeof(EventsResult), 200)]
        public async Task<IActionResult> GetList(string query, OrganisationId organisationId, EventType? eventType, string country, string[] tags, int page = 0, bool filterByUser = false)
        {
            var userId = User.GetUserId();

            var eventsQuery = new GetEvents(userId, eventType, country, tags, query, page, filterByUser, organisationId);
            var events = await _messageDispatcher.Send(eventsQuery);

            return Json(events);
        }

        [HttpGet("~/api/event/map")]
        [ProducesResponseType(typeof(EventsMapResult), 200)]
        public async Task<IActionResult> Map(string query, OrganisationId organisationId, EventType? eventType, string country, string[] tags, int page = 0, bool filterByUser = false)
        {
            var userId = User.GetUserId();

            var eventsQuery = new GetEventsMap(userId, eventType, country, tags, query, page, filterByUser, organisationId);
            var events = await _messageDispatcher.Send(eventsQuery);

            return Json(events);
        }

        [HttpGet("~/api/event/{id}/")]
        [ProducesResponseType(typeof(EventDetails), 200)]
        public async Task<IActionResult> Get(EventId id)
        {
            var userId = User.GetUserId();

            var query = new GetEvent(userId, id);
            var experience = await _messageDispatcher.Send(query);

            return experience != null ? Json(experience) : id.AcceptedWhenRecent();
        }

        [HttpGet("~/api/event/{id}/summary")]
        [ProducesResponseType(typeof(EventSummary), 200)]
        public async Task<IActionResult> Summary(EventId id)
        {
            var userId = User.GetUserId();

            var query = new GetEventSummary(userId, id);
            var @event = await _messageDispatcher.Send(query);

            return View(@event);
        }

        [HttpGet("~/api/event/{id}/members")]
        [ProducesResponseType(typeof(EventSummary), 200)]
        public async Task<IActionResult> GetMembers(EventId id)
        {
            var userId = User.GetUserId();

            var query = new GetEventMembers(userId, id);
            var result = await _messageDispatcher.Send(query);

            return Json(result);
        }

        [Authorize]
        [HttpDelete("~/api/event/{id}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Remove(EventId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveEvent(userId, id);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [HttpPut("~/api/event/{id}/report")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Report(EventId id, Description reason)
        {
            var userId = User.GetUserId();

            var query = new ReportEvent(id, userId, reason);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/update-name")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateName(EventId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeEventName(userId, id, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/update-description")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateDescription(EventId id, Description description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeEventDescription(userId, id, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/update-startdatetime")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateStartDateTime(EventId id, DateTime dateTime)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeEventStartDateTime(userId, id, dateTime);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/update-enddatetime")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateEndDateTime(EventId id, DateTime dateTime)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeEventEndDateTime(userId, id, dateTime);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/update-location")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateLocation(EventId id, Name name, GooglePlace address)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeEventLocation(userId, id, new EventLocation
            {
                Address = address.ToAddress(),
                Name = name
            });
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/update-privacy")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateEventPrivacy(EventId id, Name name, EventPrivacy privacy)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeEventPrivacy(userId, id, privacy);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/update-type")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateEventType(EventId id, Name name, EventType eventType)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeEventType(userId, id, eventType);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }


        [Authorize]
        [HttpPut("~/api/event/{id}/members/invite")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> MemberInvite(EventId id, UserId memberId)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new InviteEventMember(id, userId, memberId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/members/join")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> MemberJoin(EventId id, EventMemberStatus status)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new JoinEventMember(id, userId, status);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/event/{id}/members/{memberId}/change-status")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> MemberChangeStatus(EventId id, UserId memberId, EventMemberStatus status)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeEventMemberStatus(id, memberId, userId, status);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/event/{id}/members/{memberId}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> MemberRemove(EventId id, UserId memberId)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveEventMember(id, memberId, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        /// <summary>
        /// Add photos to an event.
        /// </summary>
        [HttpPost("~/api/event/{id}/image/")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddPhoto(EventId id, ICollection<IFormFile> files)
        {
            var userId = User.GetUserId();

            if (files.Count != 1)
            {
                return BadRequest("Invalid number of files. Should be 1.");
            }

            var formFiles = await files.Save(_configuration);

            var image = formFiles
                .Select(file => new Image(new ImageId(file.Id), userId, file.FileName, file.OriginalFileName))
                .First();

            var query = new ChangeEventImage(id, userId, image);
            var result = await _messageDispatcher.Send(query);

            return this.ResultData(result);
        }

        [Authorize]
        [HttpDelete("~/api/event/{id}/image")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemovePhoto(EventId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ClearEventImage(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/event/{id}/tags/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddTag(EventId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new AddEventTag(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/event/{id}/tags/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveTag(EventId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveEventTag(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [HttpGet("~/api/event/tags/")]
        [ProducesResponseType(typeof(Tag[]), 200)]
        public async Task<IActionResult> Tags(string query)
        {
            var userId = User.GetUserId();

            var request = new TagsQuery(userId, query, TagsDomain.Events);
            var tags = await _messageDispatcher.Send(request);

            return Json(tags);
        }
    }
}
