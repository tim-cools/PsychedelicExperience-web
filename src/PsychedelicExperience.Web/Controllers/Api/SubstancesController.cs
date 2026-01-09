using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Substances.Queries;

namespace PsychedelicExperience.Web.Controllers.Api
{
    public class SubstancesController : Controller
    {
        private readonly IMediator _messageDispatcher;

        public SubstancesController(IMediator messageDispatcher)
        {
            if (messageDispatcher == null) throw new ArgumentNullException(nameof(messageDispatcher));

            _messageDispatcher = messageDispatcher;
        }

        [HttpGet("~/api/substance/")]
        [ProducesResponseType(typeof(SubstanceResult), 200)]
        public async Task<IActionResult> Get(string query)
        {
            var userId = User.GetUserId();

            var request = new SubstancesQuery(userId, query);
            var user = await _messageDispatcher.Send(request);

            return Json(user);
        }

        [HttpGet("~/api/units/")]
        [ProducesResponseType(typeof(UnitResult), 200)]
        public async Task<IActionResult> GetUnits(string query)
        {
            var userId = User.GetUserId();

            var request = new UnitsQuery(userId, query);
            var user = await _messageDispatcher.Send(request);

            return Json(user);
        }

        [HttpGet("~/api/methods/")]
        [ProducesResponseType(typeof(MethodResult), 200)]
        public async Task<IActionResult> GetMethods(string query)
        {
            var userId = User.GetUserId();

            var request = new MethodsQuery(userId, query);
            var user = await _messageDispatcher.Send(request);

            return Json(user);
        }
    }
}
