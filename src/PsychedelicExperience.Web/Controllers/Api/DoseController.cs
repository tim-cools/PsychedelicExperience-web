using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.ViewModels.Api;

namespace PsychedelicExperience.Web.Controllers.Api
{
    public class DoseController : Controller
    {
        private readonly IMediator _messageDispatcher;

        public DoseController(IMediator messageDispatcher)
        {
            if (messageDispatcher == null) throw new ArgumentNullException(nameof(messageDispatcher));

            _messageDispatcher = messageDispatcher;
        }

        [HttpGet("~/api/dose/{id}")]
        [ProducesResponseType(typeof(Dose), 200)]
        public async Task<IActionResult> Get(DoseId id)
        {
            var userId = User.GetUserId();

            var query = new GetDose(userId, id);
            var experience = await _messageDispatcher.Send(query);

            return Json(experience);
        }

        [Authorize]
        [HttpDelete("~/api/dose/{id}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Remove(DoseId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveDose(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/dose/{id}/update-amount")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateAmount(DoseId id, decimal? amount)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateDoseAmount(userId, id, amount);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/dose/{id}/update-unit")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateAmount(DoseId id, string unit)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateDoseUnit(userId, id , unit);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/dose/{id}/update-substance")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateSubstance(DoseId id, string substance)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateDoseSubstance(userId, id, substance);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/dose/{id}/update-form")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateForm(DoseId id, string form)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateDoseForm(userId, id, form);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/dose/{id}/update-notes")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateNotes(DoseId id, string notes)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateDoseNotes(userId, id, notes);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/dose/{id}/update-method")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateMethod(DoseId id, string method)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateDoseMethod(userId, id, method);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/dose/{id}/update-timeoffset")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateTimeOffset(DoseId id, int? timeOffset)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UpdateDoseTimeOffset(userId, id, timeOffset);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }
    }
}
