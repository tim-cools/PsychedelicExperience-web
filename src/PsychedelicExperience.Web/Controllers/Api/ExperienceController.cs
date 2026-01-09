using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;

namespace PsychedelicExperience.Web.Controllers.Api
{
    [UnauthenticateWhenSessionExpired]
    public class ExperienceController : Controller
    {
        private readonly IMediator _messageDispatcher;

        public ExperienceController(IMediator messageDispatcher)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
        }

        [UnauthenticateWhenSessionExpired]
        [HttpGet("~/api/experience/")]
        [ProducesResponseType(typeof(ExperiencesResult), 200)]
        public async Task<IActionResult> GetList(string query, string[] substances, string[] tags, int page, bool filterByUser = false)
        {
            var userId = User.GetUserId();

            var experiencesQuery = new GetExperiences(userId, query, substances, tags, page, filterByUser);
            var experiences = await _messageDispatcher.Send(experiencesQuery);

            return Json(experiences);
        }

        [UnauthenticateWhenSessionExpired]
        [HttpGet("~/api/experience/statistics")]
        [ProducesResponseType(typeof(ExperienceStatistics), 200)]
        public async Task<IActionResult> Statistics()
        {
            var experiencesQuery = new GetExperienceStatistics();
            var experiences = await _messageDispatcher.Send(experiencesQuery);

            return Json(experiences);
        }

        [HttpGet("~/api/experience/{id}/")]
        [ProducesResponseType(typeof(ExperienceDetails), 200)]
        public async Task<IActionResult> Get(ExperienceId id)
        {
            var userId = User.GetUserId();

            var query = new GetExperience(userId, id);
            var experience = await _messageDispatcher.Send(query);

            return experience != null ? Json(experience) : id.AcceptedWhenRecent();
        }

        [Authorize]
        [HttpPost("~/api/experience/")]
        [ProducesResponseType(typeof(CreatedAtActionResult), 201)]
        public async Task<IActionResult> Post(ExperienceData data)
        {
            if (data == null)
            {
                throw new BusinessException("Data is not set");
            }

            var id = ExperienceId.New();
            var userId = User.GetAuthorizedUserId();

            //data.TryGetValue("title", out var title);
            //data.TryGetValue("description", out var description);

            var query = new AddExperience(userId, id, data);
            var result = await _messageDispatcher.Send(query);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return CreatedAtAction("Get", new { id = new ShortGuid(id.Value) }, null);
        }

        [Authorize]
        [HttpDelete("~/api/experience/{id}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Remove(ExperienceId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveExperience(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [HttpPut("~/api/experience/{id}/report")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Report(ExperienceId id, string reason)
        {
            var userId = User.GetUserId();

            var query = new ReportExperience(id, userId, reason);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/experience/{id}/update-title")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateTitle(ExperienceId id, Title title)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateExperienceTitle(id, userId, title);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/experience/{id}/update-datetime")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateDateTime(ExperienceId id, DateTime? dateTime)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateExperienceDateTime(id, userId, dateTime);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/experience/{id}/update-set")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateSet(ExperienceId id, Description description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateExperienceSet(id, userId, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/experience/{id}/update-setting")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateSetting(ExperienceId id, Description description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateExperienceSetting(id, userId, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/experience/{id}/update-public-description")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdatePublic(ExperienceId id, Description description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateExperiencePublicDescription(id, userId, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/experience/{id}/update-private-notes")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdatePrivateNotes(ExperienceId id, Description description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateExperiencePrivateNotes(id, userId, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/experience/{id}/update-privacy-level")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdatePrivacyLevel(ExperienceId id, PrivacyLevel level)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new SetExperiencePrivacy(id, userId, level);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/experience/{id}/update-experience-level")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateExperienceLevel(ExperienceId id, ExperienceLevel level)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new SetExperienceLevel(id, userId, level);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/experience/{id}/tags/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddTag(ExperienceId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new AddExperienceTag(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/experience/{id}/tags/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveTag(ExperienceId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveExperienceTag(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/experience/{id}/dose")]
        [ProducesResponseType(typeof(CreatedAtActionResult), 201)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> PostDose(ExperienceId id)
        {
            var doseId = DoseId.New();
            var userId = User.GetAuthorizedUserId();

            var query = new AddDose(id, userId, doseId);
            var result = await _messageDispatcher.Send(query);
            if (!result.Succeeded) return BadRequest();

            return CreatedAtAction("Get", "Dose", new { id = doseId }, null);
        }

        [HttpGet("~/api/experience/tags/")]
        [ProducesResponseType(typeof(Tag[]), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Tags(string query)
        {
            var userId = User.GetUserId();

            var request = new TagsQuery(userId, query, TagsDomain.Experiences);
            var tags = await _messageDispatcher.Send(request);

            return Json(tags);
        }
    }
}
